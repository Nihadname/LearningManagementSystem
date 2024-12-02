﻿using LearningManagementSystem.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LearningManagementSystem.Application.Exceptions;

namespace LearningManagementSystem.Application.Implementations
{
    public class EmailService : IEmailService
    {
        public void SendEmailOld(List<string> emails, string body, string title, string subject)
        {
            MailMessage mail = new();
            mail.From = new MailAddress("tahiraa@code.edu.az", "PlaystationAze");
            foreach (var email in emails)
            {
                mail.To.Add(new MailAddress(email));
            }
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = body;

            SmtpClient smtpClient = new()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential("tahiraa@code.edu.az", "blcf yubd mxnb gcyb"),
            };
            smtpClient.Send(mail);
        }
        public void SendEmail(string from, string to, string subject, string body, string smtpHost, int smtpPort, bool enableSsl, string smtpUser, string smtpPass)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(from);
                mailMessage.To.Add(new MailAddress(to));
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = body;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = smtpHost;
                smtpClient.Port = smtpPort;
                smtpClient.EnableSsl = enableSsl;
                smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPass);
                smtpClient.Send(mailMessage);
            }
            catch (SmtpFailedRecipientException ex)
            {
                throw new CustomException(400,ex.Message);
            }
        }

        public async Task SendEmailAsyncToManyPeople(string from, List<string> recipients, string subject, string body, string smtpHost, int smtpPort, bool enableSsl, string smtpUser, string smtpPass)
        {
            foreach (var to in recipients)
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(from);
                mailMessage.To.Add(new MailAddress(to));
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = body;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = smtpHost;
                smtpClient.Port = smtpPort;
                smtpClient.EnableSsl = enableSsl;
                smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPass);

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}