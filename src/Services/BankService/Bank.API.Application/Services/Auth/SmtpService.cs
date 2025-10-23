using Bank.API.Application.DTOs.Users;
using Bank.API.Application.ServiceContracts.BankServiceContracts.Auth;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Bank.API.Application.Services.Auth
{
    public class SmtpService: ISmtpService
    {
        private readonly SmtpSettings _smtp;

        public SmtpService(IOptions<SmtpSettings> smtp)
        {
            _smtp = smtp.Value;
        }

        public async Task SendAsync(string to, string subject, string html)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_smtp.From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html) { Text = html };

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtp.Host, _smtp.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtp.User, _smtp.Pass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
