using DownloaderVideo.Domain.Entity;
using DownloaderVideo.Domain.Interface.Services.v1;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using YoutubeExplode;
using YoutubeExplode.Channels;

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

            // Executável sem caminho absoluto para Linux (presume que está no PATH)
            // Executável com caminho relativo para Windows (exemplo: pasta tools dentro do base dir)
            string ytDlpPath;
            string ffmpegLocation;

            if (isLinux)
            {
                ytDlpPath = "yt-dlp";
                ffmpegLocation = "/usr/bin";

                // Obtenha o diretório home do usuário atual.
                // O yt-dlp por padrão procura o .netrc no diretório home.
                // Se você armazenar em outro local, use --netrc-file /path/to/my/.netrc
                string userHomeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string netrcFilePath = Path.Combine(userHomeDirectory, ".netrc");

                // Verificar se o arquivo .netrc existe e se você quer usar a autenticação.
                // Você pode ter uma configuração para habilitar/desabilitar autenticação ou
                // verificar se a URL exige login.
                bool requiresAuth = true; // Ou determine isso com base na URL/configuração

                if (requiresAuth)
                {
                    // Importante: yt-dlp procurará por ".netrc" por padrão no diretório home.
                    // Se você quiser especificar um caminho diferente, use --netrc-file
                    arguments = $"--no-progress --ffmpeg-location \"{ffmpegLocation}\" --netrc {arguments}";

                    // Ou, se precisar especificar um caminho:
                    // arguments = $"--no-progress --ffmpeg-location \"{ffmpegLocation}\" --netrc-file \"{netrcFilePath}\" {arguments}";

                    // NOTA: Certifique-se de que o usuário da sua aplicação em produção
                    // tenha permissão de leitura para o arquivo .netrc e que ele esteja
                    // seguro (chmod 600).
                }
                else
                {
                    // Para vídeos que não exigem autenticação
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
