namespace DownloaderVideo.Domain.Interface.Services.v1;

public interface IEmailService
{
    Task SendMailAsync(string from, string email, string subject, string message);
}
