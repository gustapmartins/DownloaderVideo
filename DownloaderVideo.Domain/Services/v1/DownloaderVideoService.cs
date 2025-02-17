using DownloaderVideo.Domain.Entity;
using DownloaderVideo.Domain.Interface.Services.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DownloaderVideo.Application.Controllers.v1;

public class DownloaderVideoService : IDownloaderVideoService
{

    private readonly IConfiguration _configuration;
    private readonly INotificationBase _notificationBase;

    public DownloaderVideoService(
        IConfiguration configuration,
        INotificationBase notificationBase)
    {
        _configuration = configuration;
        _notificationBase = notificationBase;
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

            // Buscar arquivos que contenham o nome fornecido, ignorando maiúsculas/minúsculas
            string filePath = Directory.GetFiles(tempDirectory)
                .FirstOrDefault(f => Path.GetFileName(f).Contains(fileName, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(filePath))
            {
                await _notificationBase.NotifyAsync($"Nome do arquivo: {fileName}", "Arquivo não encontrado.");
                return null;
            }

            return new OperationResult<string>()
            {
                Content = filePath.Trim(),
                Message = "Video encontrado com sucesso",
                Status = true,
                StatusCode = StatusCodes.Status200OK
            };
        }
        catch (Exception ex)
        {
            return ResponseObject<string>(string.Empty, $"Erro ao baixar o vídeo: {ex.Message}", StatusCodes.Status500InternalServerError, false);
        }
    }

    public async Task<OperationResult<string>> DownloadVideo(string url, string quality)
    {
        try
        {
            string ytDlpPath = @"C:\yt-dlp\yt-dlp.exe"; // Caminho do yt-dlp

            // Obtém o diretório temporário do sistema
            string tempDirectory = Path.GetTempPath();

            // Define o caminho de saída do arquivo na pasta de vídeos
            string outputFilePath = Path.Combine(tempDirectory, $"{Guid.NewGuid()}.mp4");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ytDlpPath,
                    Arguments = $"-f \"{quality}+bestaudio\" -o \"{outputFilePath}\" \"{url}\"",
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

            string downloadLink = $"http://localhost:7155/api/v1/DownloaderVideo/GetDownload/{Path.GetFileName(outputFilePath)}";

            return new OperationResult<string>() {
                Content = downloadLink.Trim(),
                Message = "Download concluido com sucesso",
                Status = true,
                StatusCode = StatusCodes.Status200OK
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao obter a URL do vídeo: {ex.Message}");
        }
    }

    public OperationResult<List<DownloaderVideoEntity>> GetAvailableQualities(string url)
    {
        try
        {
            string output = RunYtDlp(url);

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
                                Resolution = resolution
                            };
                        }
                    }
                }
            }

            var qualities = uniqueResolutions.Values
                .OrderBy(q => int.Parse(Regex.Replace(q.Resolution, @"[^\d]", "")))
                .ToList();

            return ResponseObject(qualities, "Qualidades disponíveis (MP4 - H.264)", StatusCodes.Status200OK, true);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar as qualidades: {ex.Message}");
        }
    }

    #region Metodos privados 
    private string RunYtDlp(string url)
    {
        try
        {
            string ytDlpPath = @"C:\Users\gusta\Downloads\yt-dlp.exe";  // Altere conforme necessário

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ytDlpPath,
                    Arguments = $"--list-formats \"{url}\"",  // Listar formatos disponíveis
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

            return output; // Retorna os formatos disponíveis

        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao executar yt-dlp: {ex.Message}");
        }
    }

    private OperationResult<T> ResponseObject<T>(T content, string message, int statusCode, bool status)
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
