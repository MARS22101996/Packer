using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using NotificationService.BLL.Interfaces;

namespace NotificationService.BLL.Infrastructure
{
    public class EmailSender : IEmailSender
    {
        private readonly string _mailboxName;
        private readonly string _mailboxAddress;
        private readonly string _mailboxPassword;
        private readonly string _host;
        private readonly int _port;
        private readonly bool _enableSsl;

        public EmailSender(IConfiguration configuration)
        {
            _mailboxName = configuration["MailData:Name"];
            _mailboxAddress = configuration["MailData:Address"];
            _mailboxPassword = configuration["MailData:Password"];
            _host = configuration["MailData:Host"];
            _port = int.Parse(configuration["MailData:Port"]);
            _enableSsl = Convert.ToBoolean(configuration["MailData:EnableSsl"]);
        }

        public async Task SendAsync(IEnumerable<string> emails, string message, string subject)
        {
            var msg = new MimeMessage();

            msg.From.Add(new MailboxAddress(_mailboxName, _mailboxAddress));
            msg.To.AddRange(emails.Select(s => new MailboxAddress(s)));

            msg.Subject = subject;
            msg.Body = new TextPart("plain") { Text = message };

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_host, _port, _enableSsl);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_mailboxAddress, _mailboxPassword);
                    await client.SendAsync(msg);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception) 
            {
                // We can`t catch StackOverflowException, so we catch Exception 
                // http://stackoverflow.com/questions/1599219/c-sharp-catch-a-stack-overflow-exception
                // Ignored. If user email invlid, we just ignore it.
            }
        }
    }
}