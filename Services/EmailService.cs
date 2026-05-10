using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Services.Helpers;

namespace Services;

public class EmailService(IOptions<SmtpAddressOptions> smtpAddressOptions, IOptions<SmtpAuthOptions> smtpAuthOptions)
{
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Seawave Support", "support@seawave.local"));
        message.To.Add(new MailboxAddress("Seawave User", toEmail));
        message.Subject = subject;
        
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = body,
            TextBody = "Please use an HTML-compatible email client to view this message."
        };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(smtpAddressOptions.Value.Host, smtpAddressOptions.Value.Port, 
            MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(smtpAuthOptions.Value.Username, smtpAuthOptions.Value.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}