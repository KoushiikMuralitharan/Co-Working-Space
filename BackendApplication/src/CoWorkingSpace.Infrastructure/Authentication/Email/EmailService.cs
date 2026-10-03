using System;
using System.Collections.Generic;
using System.Text;
using CoWorkingSpace.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;


namespace CoWorkingSpace.Infrastructure.Authentication.Email
{
    public class EmailService : IEmailService
    {

        private IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public async Task SendEmailAsync( string to, string subject, string body)
        {
            var smtpServer = _configuration["Email:SmtpServer"]
                                ?? throw new InvalidOperationException(
                                        "Email SMTP server is not configured."
                                    );
            var smtpPort = int.Parse(
                _configuration["Email:SmtpPort"]
                ?? throw new InvalidOperationException(
                    "Email SMTP port is not configured."));

            var username =
            _configuration["Email:Username"]
            ?? throw new InvalidOperationException(
                "Email username is not configured.");

            var password =
                _configuration["Email:Password"]
                ?? throw new InvalidOperationException(
                    "Email password is not configured.");

            var from =
                _configuration["Email:From"]
                ?? throw new InvalidOperationException(
                    "Email sender is not configured.");

            var message = new MimeMessage();

            message.From.Add(
                MailboxAddress.Parse(from));

            message.To.Add(
                MailboxAddress.Parse(to));

            message.Subject = subject;

            message.Body =
            new BodyBuilder
            {
                HtmlBody = body
            }.ToMessageBody();

            using var client = new SmtpClient();

            await client.ConnectAsync(
                smtpServer,
                smtpPort,
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                username,
                password);

            await client.SendAsync(message);

            await client.DisconnectAsync(true);
        }
    }
}
