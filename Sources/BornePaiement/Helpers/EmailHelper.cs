using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


//Ce code ne vient pas de moi mais d'un travail précédent fait avec Jerome et Xavier
namespace BornePaiement.Helpers
{
    public static class EmailHelper
    {

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
