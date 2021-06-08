using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using TCC.Models;

namespace TCC.Libs.EmailContato
{
    public class ContatoEmail
    {
        private readonly SmtpClient _smtClient;
        private readonly string _email;

        public ContatoEmail(IConfiguration config)
        {
            string host = config["emailCredentials:host"];
            int port = Convert.ToInt32(config["emailCredentials:port"]);
            _email = config["emailCredentials:email"];
            string password = config["emailCredentials:password"];
            _smtClient = new SmtpClient(host, port);
            _smtClient.Credentials = new NetworkCredential(_email, password);
            _smtClient.EnableSsl = true;
        }
        public bool EnviarEmail(MensagemEmail mensagem)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_email);
            mailMessage.To.Add(mensagem.Destinatario);
            mailMessage.Subject = mensagem.Titulo;
            mailMessage.Body = mensagem.Conteudo;
            mailMessage.IsBodyHtml = mensagem.Html;
            try
            {
                _smtClient.Send(mailMessage);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
