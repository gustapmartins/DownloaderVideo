using DownloaderVideo.Domain.Interface.Services.v1;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using System.Diagnostics.CodeAnalysis;

namespace DownloaderVideo.Domain.Services.v1;

[ExcludeFromCodeCoverage]
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendMailAsync(string from, string email, string subject, string message)
    {
        string mail = _configuration["EmailTrap:Email"];

        SmtpClient client = new(_configuration["EmailTrap:Host"], int.Parse(_configuration["EmailTrap:Port"]))
        {
            Credentials = new NetworkCredential(mail, _configuration["EmailTrap:Password"]),
            EnableSsl = true,
            UseDefaultCredentials = false
        };

        try
        {
            MailMessage mailMessage = new(from, email, subject, message)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);
        }
        catch (OperationCanceledException)
        {
            // Handle the cancellation exception if the operation was canceled.
            Console.WriteLine("Email sending was canceled.");
        }
        finally
        {
            client.Dispose();
        }
    }
}
