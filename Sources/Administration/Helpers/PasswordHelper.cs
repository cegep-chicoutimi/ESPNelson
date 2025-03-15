using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Administration.Data;
using Administration.Data.Context;
using Administration.Model;
using Administration.Resources;


//Ce code ne vient pas de moi mais d'un travail précédent fait avec Jerome et Xavier
namespace Administration.Helpers
{
    public class PasswordHelper
    {
        /// <summary>
        /// Génère un mot de passe aléatoire de la longueur spécifiée. Contiens les 26 lettres de l'alphabet en majuscule et minuscule ainsi que les chiffres de 0 à 9.
        /// </summary>
        /// <param name="length">La longueur voulu du mot de passe</param>
        /// <returns>Un mot de passe aléatoire de la longueur spécifié</returns>
        public static async Task<string> GeneratePassword(int length)
        {
            string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                sb.Append(validChars[rnd.Next(validChars.Length)]);
            }
            return sb.ToString();
        }


        /// <summary>
        /// Réinitialise le mot de passe d'un utilisateur et lui envoie un nouveau mot de passe par email.
        /// </summary>
        /// <param name="email">L'adresse email de l'utilisateur pour lequel réinitialiser le mot de passe.</param>
        /// <param name="utilisateur">L'utilisateur dont le mot de passe doit être réinitialisé (optionnel).</param>
        /// <param name="context">Le contexte de base de données (optionnel).</param>
        /// <returns>
        /// <c>true</c> si la réinitialisation du mot de passe et l'envoi de l'email ont réussi ;
        /// <c>false</c> en cas d'erreur (email invalide, échec d'envoi d'email, ou erreur de base de données).
        /// </returns>
        public static async Task<bool> ResetPassword(string email, Utilisateur? utilisateur = null, AdministrationContext? context = null)
        {
            // Verify that the email is valid
            if (email is null || !EmailHelper.IsValidEmail(email))
            {
                MessageBox.Show(
                     Resource.InvalidEmail,
                     Resource.ErrorTitle,
                     MessageBoxButton.OK,
                     MessageBoxImage.Error
                );
                return false;
            }

            // Generate a new password
            string newPassword = await GeneratePassword(8);

            // Send the new password to the user
            string subject = Resource.PasswordResetSubject;
            string body = string.Format(Resource.PasswordResetBody, newPassword);
            await EmailHelper.SendEmail(string.Empty, email, subject, body);

            // Update the password in the database
            if (utilisateur is null || context is null)
            {
                AdministrationContextFactory factory = new AdministrationContextFactory();
                context = factory.CreateDbContext(new string[0]);
                utilisateur = context.Utilisateurs.FirstOrDefault(u => u.Email == email);
            }
            utilisateur.MotDePasse = CryptographyHelper.HashPassword(newPassword);
            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {

                MessageBox.Show(
                    string.Format(Resource.PasswordResetError, e.Message),
                    Resource.ErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return false;
            }
            return true;
        }
    }
}
