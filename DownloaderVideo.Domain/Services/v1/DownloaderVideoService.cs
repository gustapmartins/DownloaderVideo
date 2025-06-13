using System.Diagnostics;
using System.Text.RegularExpressions;
using DownloaderVideo.Domain.Entity;
using DownloaderVideo.Domain.Interface.Services.v1;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using YoutubeExplode;

namespace DownloaderVideo.Application.Controllers.v1;

public class DownloaderVideoService(
    INotificationBase _notificationBase, 
    YoutubeClient _youtubeClient) : IDownloaderVideoService
{
    public OperationResult<Stream> DownloadVideo(string url, string quality)
    {
        try
        {
            string arguments = $"-f \"{quality}+bestaudio\" -o - \"{url}\"";

            Stream output = RunYtDlp<Stream>(arguments);

            return ResponseObject(output, $"Video baixado com sucesso", true, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseObject(Stream.Null, $"Erro ao obter a URL do vídeo: {ex.Message}", true, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<OperationResult<List<DownloaderVideoEntity>>> GetAvailableQualitiesAsync(string url) 
    {
        try
        {
            string arguments = $"--list-formats \"{url}\"";

            string output = RunYtDlp<string>(arguments);

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

            var filename = await _youtubeClient.Videos.GetAsync(url);
            string videoTitle = filename.Title.Replace(" ", "_").Replace("|", "").Replace("\"", "").Replace("'", "").Replace("/", "").Replace("\\", "");

            return ResponseObject(qualities, videoTitle, true, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseObject(new List<DownloaderVideoEntity>(), $"Erro ao listar as qualidades: {ex.Message}", false, StatusCodes.Status500InternalServerError);
        }
    }

    #region Metodos privados

    private T RunYtDlp<T>(string arguments)
    {
        try
        {
            bool isLinux = OperatingSystem.IsLinux();

            string ytDlpPath;
            string ffmpegLocation;

            if (isLinux)
            {
                Console.WriteLine("DEBUG: Entrou no bloco isLinux."); // *** Novo log ***

                ytDlpPath = "yt-dlp"; // ou: Path.Combine(AppContext.BaseDirectory, "tools", "yt-dlp");
                ffmpegLocation = "/usr/bin";

                string cookiesPath = Path.Combine(AppContext.BaseDirectory, "cookies.txt");

                Console.WriteLine($"cookies Path: {cookiesPath}, {AppContext.BaseDirectory}");

                if (File.Exists(cookiesPath))
                {
                    Console.WriteLine($"cookies Path: {cookiesPath}, dentro do arquivo existente");
                    arguments = $"--no-progress --ffmpeg-location \"{ffmpegLocation}\" --cookies \"{cookiesPath}\" {arguments}";
                }
                else
                {
                    arguments = $"--no-progress --ffmpeg-location \"{ffmpegLocation}\" {arguments}";
                }
            }
            else
            {
                var basePath = Path.Combine(AppContext.BaseDirectory, "tools");
                ytDlpPath = Path.Combine(basePath, "yt-dlp.exe");
                ffmpegLocation = basePath;

                if (!File.Exists(ytDlpPath))
                    throw new FileNotFoundException($"Arquivo não encontrado: {ytDlpPath}");

                arguments = $"--no-progress --ffmpeg-location \"{ffmpegLocation}\" {arguments}";
            }

            var processStartInfo = new ProcessStartInfo
            {
                FileName = ytDlpPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = processStartInfo };
            process.Start();

            if (typeof(T) == typeof(Stream))
            {
                return (T)(object)process.StandardOutput.BaseStream;  // Retorna Stream
            }
            else if (typeof(T) == typeof(string))
            {
                string output = process.StandardOutput.ReadToEnd();
                string errorOutput = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(errorOutput))
                {
                    throw new Exception($"Erro no yt-dlp: {errorOutput}");
                }

                return (T)(object)output.Trim();  // Retorna string
            }
            else
            {
                throw new InvalidOperationException("Tipo de retorno inválido.");
            }
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
