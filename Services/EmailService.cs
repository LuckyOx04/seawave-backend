using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Services;

public class EmailService(IConfiguration config)
{
    private readonly string _host = "sandbox.smtp.mailtrap.io";
    private readonly int _port = 2525;
    private readonly string _username = config["Smtp:Username"] ?? "";
    private readonly string _password = config["Smtp:Password"] ?? "";

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Seawave Support", "support@seawave.local"));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(_host, _port, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_username, _password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}