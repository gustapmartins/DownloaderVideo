using DownloaderVideo.Domain.Entity;
using DownloaderVideo.Domain.Interface.Services.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Diagnostics;
using System.Text.RegularExpressions;
using YoutubeExplode;

namespace DownloaderVideo.Application.Controllers.v1;

public class DownloaderVideoService : IDownloaderVideoService
{
    private readonly INotificationBase _notificationBase;
    private readonly YoutubeClient _youtubeClient;

    public DownloaderVideoService(
        INotificationBase notificationBase)
    {
        _notificationBase = notificationBase;
        _youtubeClient = new YoutubeClient();
    }

    public async Task<OperationResult<string>> GetVideoDownloadUrl(string fileName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(fileName) || fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
            {
                await _notificationBase.NotifyAsync($"Nome do arquivo: {fileName}", "Nome do arquivo inválido.");
                return null;
            }

            string tempDirectory = Path.GetTempPath();

            string filePath = Directory.GetFiles(tempDirectory)
                .FirstOrDefault(f => Path.GetFileName(f).Contains(fileName, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(filePath))
            {
                await _notificationBase.NotifyAsync($"Nome do arquivo: {fileName}", "Arquivo não encontrado.");
                return null;
            }

            return ResponseObject(filePath.Trim(), "Video encontrado com sucesso", true, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseObject(string.Empty, $"Erro ao baixar o vídeo: {ex.Message}", false, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<OperationResult<string>> DownloadVideo(string url, string quality)
    {
        try
        {
            var filename = await _youtubeClient.Videos.GetAsync(url);

            string videoTitle = filename.Title.Replace(" ", "_").Replace("|", "").Replace("\"", "").Replace("'", "").Replace("/", "").Replace("\\", ""); // Limpeza do nome

            string outputFilePath = Path.Combine(Path.GetTempPath(), $"{videoTitle}.mp4");

            string arguments = $"-f \"{quality}+bestaudio\" -o \"{outputFilePath}\" \"{url}\"";

            string output = RunYtDlp(arguments);

            return ResponseObject(videoTitle.Trim(), $"Video baixado com sucesso", true, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseObject(string.Empty, $"Erro ao obter a URL do vídeo: {ex.Message}", true, StatusCodes.Status500InternalServerError);
        }
    }

    public OperationResult<List<DownloaderVideoEntity>> GetAvailableQualities(string url)
    {
        try
        {
            string arguments = $"--list-formats \"{url}\"";

            string output = RunYtDlp(arguments);

            var uniqueResolutions = new Dictionary<string, DownloaderVideoEntity>();

            var lines = output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                // Regex para capturar ID, formato, resolução e codec
                var match = Regex.Match(line, @"(?<Id>\d+)\s+(?<Format>\w+)\s+(?<Resolution>\d{3,4}x\d{3,4})\s+(?<Codec>\w+)");

                if (match.Success)
                {
                    string id = match.Groups["Id"].Value;
                    string format = match.Groups["Format"].Value;
                    string resolution = match.Groups["Resolution"].Value;
                    string codec = match.Groups["Codec"].Value;

                    // Filtrar apenas os vídeos do formato 'webm_dash'
                    if (format.Contains("webm"))
                    {
                        if (!uniqueResolutions.ContainsKey(resolution))
                        {
                            uniqueResolutions[resolution] = new DownloaderVideoEntity
                            {
                                Id = id,
                                Resolution = resolution,
                                Codec = codec,
                                Format = format
                            };
                        }
                    }
                }
            }

            var qualities = uniqueResolutions.Values
                .OrderBy(q => int.Parse(Regex.Replace(q.Resolution, @"[^\d]", "")))
                .ToList();

            return ResponseObject(qualities, "Qualidades disponíveis (MP4 - H.264)", true, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseObject(new List<DownloaderVideoEntity>(), $"Erro ao listar as qualidades: {ex.Message}", false, StatusCodes.Status500InternalServerError);
        }
    }

    #region Metodos privados 
    private string RunYtDlp(string arguments)
    {
        try
        {
            string ytDlpPath = @"C:\yt-dlp\yt-dlp.exe";  // Altere conforme necessário

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ytDlpPath,
                    Arguments = arguments,  // Listar formatos disponíveis
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string errorOutput = process.StandardError.ReadToEnd();  // Captura a saída de erro
            process.WaitForExit();

            if (!string.IsNullOrEmpty(errorOutput))
            {
                throw new Exception($"Erro no yt-dlp: {errorOutput}");
            }

            return output;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao executar yt-dlp: {ex.Message}");
        }
    }

    private OperationResult<T> ResponseObject<T>(T content, string message, bool status, int statusCode)
    {
        return new OperationResult<T>()
        {
            Content = content,
            Message = message,
            StatusCode = statusCode,
            Status = status
        };
    }

    #endregion
}
