using MailKit.Net.Smtp;
using MimeKit;
using UserService.Application.Common;
using Microsoft.Extensions.Configuration;

namespace UserService.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config["Email:From"]));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart("plain") { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_config["Email:SmtpHost"], int.Parse(_config["Email:SmtpPort"]), false);
        await smtp.AuthenticateAsync(_config["Email:Username"], _config["Email:Password"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
