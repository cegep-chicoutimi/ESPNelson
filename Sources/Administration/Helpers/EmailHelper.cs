using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


//Ce code ne vient pas de moi mais d'un travail précédent fait avec Jerome et Xavier
namespace Administration.Helpers
{
    public static class EmailHelper
    {
        /// <summary>
        /// Envoie un courriel à partir de l'adresse spécifiée vers l'adresse spécifiée via un serveur SMTP
        /// </summary>
        /// <param name="fromEmail"></param>
        /// <param name="toEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async static Task SendEmail(string fromEmail, string toEmail, string subject, string body)
        {
            // Create an email message
            // On doit utiliser un compte gmail pour envoyer des courriels gratuitement avec SmtpClient
            fromEmail = "jeromesavardflam@gmail.com";
            MailMessage email = await CreateMail(fromEmail, toEmail, subject, body);

            // Create an SMTP client
            // On utilise le serveur smtp de gmail pour envoyer le courriel
            string host = "smtp.gmail.com";
            int port = 587;
            string password = "qczv xtnj uoto kwux"; // App password for gmail
            SmtpClient client = await CreateSMTPClient(host, port, fromEmail, password);

            // Send the email
            await client.SendMailAsync(email);
        }

        /// <summary>
        /// Cette méthode crée un objet MailMessage à partir des paramètres fournis
        /// </summary>
        /// <param name="fromEmail">Le courriel de l'envoyeur. Normallement, il serait le nom de domaine ou le mail du la companie.</param>
        /// <param name="toEmail">Le courriel du destinataire.</param>
        /// <param name="subject">Le sujet (titre) du courriel.</param>
        /// <param name="body">Le corps (l'intérieur) du courriel. C'est ici que les informations devrait être.</param>
        /// <returns>L'object courriel formatté</returns>
        private async static Task<MailMessage> CreateMail(string fromEmail, string toEmail, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(fromEmail);
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = body;

            return mail;
        }

        /// <summary>
        /// Cette méthode crée un objet SmtpClient à partir des paramètres fournis
        /// </summary>
        /// <param name="host">Le fournisseur SMTP (ex : smtp.gmail.com).</param>
        /// <param name="port">Le port du client SMTP. Normallement, c'est le 587.</param>
        /// <param name="email">Le courriel de l'envoyeur.</param>
        /// <param name="password">Le mot de passe du compte de l'envoyeur.</param>
        /// <returns>Le client SMTP</returns>
        private async static Task<SmtpClient> CreateSMTPClient(string host, int port, string email, string password)
        {
            SmtpClient client = new SmtpClient(host, port);
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(email, password);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            return client;
        }

        /// <summary>
        /// Vérifie si le courriel est valide via une expression régulière (regex). La structure du courriel doit être: [nom]@[domaine].[extension]
        /// </summary>
        /// <returns>True si la structure du string ressemble à un email, sinon false</returns>
        public static bool IsValidEmail(string email)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            return regex.IsMatch(email);
        }
    }
}
