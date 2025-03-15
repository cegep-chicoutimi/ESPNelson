using System;
using System.Security.Cryptography;


//Ce code ne vient pas de moi mais d'un travail précédent fait avec Jerome et Xavier
namespace Administration.Helpers
{
    public static class CryptographyHelper
    {
        /// <summary>
        /// Hashe un mot de passe en utilisant un algorithme de hashage (salt)
        /// </summary>
        /// <param name="passwordToHash">Le mot de passe à encrypter</param>
        /// <returns>Le mot de passe encrypter</returns>
        public static string HashPassword(string passwordToHash)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            var pdkdf2 = new Rfc2898DeriveBytes(passwordToHash, salt, 4855);
            byte[] hash = pdkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Valide un mot de passe hashé avec un mot de passe en clair
        /// </summary>
        /// <param name="password">Le mot de passe non-encrypter</param>
        /// <param name="hashedPassword">Le mot de passe hashé</param>
        /// <returns>True si le mot de passe correspond. Sinon false</returns>
        public static bool ValidateHashedPassword(string password, string hashedPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 4855);
            byte[] hash = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
