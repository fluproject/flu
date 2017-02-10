using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace flu
{
    /// <summary>
    /// Clase para enviar correos
    /// </summary>
    class Mail
    {
        SmtpClient server;
        string mailVictima;
        string mailFake;
        string text;
        string subject;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mailServer">Servidor de correo</param>
        /// <param name="emailOrigen">Correo origen</param>
        /// <param name="pass">Contraseña del correo origen</param>
        /// <param name="emailVictima">Correo destino</param>
        /// <param name="emailFake">Correo (phising) o texto que aparecerá como emisor del mensaje</param>
        /// <param name="asunto">Asunto del correo</param>
        /// <param name="texto">Texto del correo</param>
        public Mail(string mailServer, string emailOrigen, string pass, string emailVictima, string emailFake, string asunto, string texto)
        {
            server = new SmtpClient(mailServer);
            server.Credentials = new System.Net.NetworkCredential(emailOrigen, pass);
            server.EnableSsl = true;
            mailVictima = emailVictima;
            mailFake = emailFake;
            text = texto;
            subject = asunto;
        }
        
        /// <summary>
        /// Envía un email
        /// </summary>
        /// <param name="n">Número de correos a enviar</param>
        public void sendMail(int n)
        {
            MailMessage mnsj = new MailMessage();
            mnsj.Subject = subject;
            mnsj.To.Add(new MailAddress(mailVictima));
            mnsj.From = new MailAddress(mailFake, mailFake);
            mnsj.Body = text;
            for (int i = 0; i < n; i++)
            {
                server.Send(mnsj);
            }
        }
    }
}