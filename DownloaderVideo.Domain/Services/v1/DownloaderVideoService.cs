using DownloaderVideo.Domain.Entity;
using DownloaderVideo.Domain.Exceptions;
using DownloaderVideo.Domain.Interface.Dao;
using DownloaderVideo.Domain.Interface.Services.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using YoutubeExplode;

namespace DownloaderVideo.Application.Controllers.v1;

public class DownloaderVideoService : IDownloaderVideoService
{

    private readonly IConfiguration _configuration;
    private readonly IDownloaderVideoDao _generateTemplateDao;
    private readonly YoutubeClient _youtubeClient;

    public DownloaderVideoService(
        IConfiguration configuration, 
        IDownloaderVideoDao generateTemplateDao)
    {
        _configuration = configuration;
        _generateTemplateDao = generateTemplateDao;
        _youtubeClient = new YoutubeClient();
    }

    public async Task<OperationResult<byte[]>> DownloadVideo(string url, string quality)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return ResponseObject<byte[]>(null!, "URL inválida.", StatusCodes.Status400BadRequest, false);
        }

        try
        {
            string downloadUrl = GetVideoDownloadUrl(url, quality);

            using (var client = new WebClient())
            {
                byte[] fileData = await client.DownloadDataTaskAsync(downloadUrl);

                var informationVideo = await _youtubeClient.Videos.GetAsync(url);

                return ResponseObject(fileData, $"Download concluído: {informationVideo.Title}.mp4", StatusCodes.Status200OK, true);
            }
        }
        catch (Exception ex)
        {
            return ResponseObject<byte[]>(null!, $"Erro ao baixar o vídeo: {ex.Message}", StatusCodes.Status500InternalServerError, false);
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

                    // Filtrar apenas vídeos MP4 com codec H.264 (avc1) e resoluções desejadas
                    if (format.Contains("mp4"))
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


    private string GetVideoDownloadUrl(string url, string quality)
    {
        try
        {
            string ytDlpPath = @"C:\Users\gusta\Downloads\yt-dlp.exe";  // Altere conforme necessário

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ytDlpPath,
                    Arguments = $"-f {quality} --get-url \"{url}\"",
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

            return output.Trim(); // Retorna a URL do vídeo
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao obter a URL do vídeo: {ex.Message}");
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
