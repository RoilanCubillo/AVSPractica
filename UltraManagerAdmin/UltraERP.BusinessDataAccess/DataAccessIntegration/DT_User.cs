using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;
using RetailHero.POS.Core.Shared;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_User : DT
    {
        #region Vars
        #endregion

        #region Constructor
        public DT_User() : base() { }
        #endregion

        #region Methods
        public (EN_User, int) ValidateUser(string userName, string password)
        {
            int count = 0;

            EN_User user = null;
            string userPassword = "";
         var resultado = db.UEP_USER_GET_BY_ACCOUNT(userName);

            foreach (var u in resultado)
            {
                user = new EN_User()
                {
                    ID = u.ID,
                    Account = u.Account,
                    EmailAddress = u.EmailAddress ?? string.Empty,
                    Name = u.UserName,
                    SecurityLevel = u.SecurityLevel,
                    UserPrivileges = u.UserPrivileges
                };

                userPassword = u.Password;

                count++;
            }

            if (count == 1)
            {
                if (IsPasswordMatch(userPassword, password)) return (user, 0);
                else return (null, 2);
            }
            else return (null, 1);
        }

        private static bool IsPasswordMatch(string storedPassword, string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(storedPassword) || plainPassword == null)
            {
                return false;
            }

            if (string.Equals(storedPassword, plainPassword, StringComparison.Ordinal))
            {
                return true;
            }

            if (TryMatchLegacyEncryptedPassword(storedPassword, plainPassword))
            {
                return true;
            }

            return MatchesSha256Base64(storedPassword, plainPassword)
                || MatchesSha256Hex(storedPassword, plainPassword);
        }

        private static bool TryMatchLegacyEncryptedPassword(string storedPassword, string plainPassword)
        {
            try
            {
                return string.Equals(
                    Cryptographer.Decrypt(storedPassword),
                    plainPassword,
                    StringComparison.Ordinal);
            }
            catch
            {
                return false;
            }
        }

        private static bool MatchesSha256Base64(string storedPassword, string plainPassword)
        {
            string utf8Hash = Convert.ToBase64String(ComputeSha256(Encoding.UTF8.GetBytes(plainPassword)));
            string unicodeHash = Convert.ToBase64String(ComputeSha256(Encoding.Unicode.GetBytes(plainPassword)));

            return string.Equals(storedPassword, utf8Hash, StringComparison.Ordinal)
                || string.Equals(storedPassword, unicodeHash, StringComparison.Ordinal);
        }

        private static bool MatchesSha256Hex(string storedPassword, string plainPassword)
        {
            string utf8Hash = ConvertToHex(ComputeSha256(Encoding.UTF8.GetBytes(plainPassword)));
            string unicodeHash = ConvertToHex(ComputeSha256(Encoding.Unicode.GetBytes(plainPassword)));

            return string.Equals(storedPassword, utf8Hash, StringComparison.OrdinalIgnoreCase)
                || string.Equals(storedPassword, unicodeHash, StringComparison.OrdinalIgnoreCase);
        }

        private static byte[] ComputeSha256(byte[] input)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(input);
            }
        }

        private static string ConvertToHex(byte[] bytes)
        {
            var builder = new StringBuilder(bytes.Length * 2);

            foreach (byte value in bytes)
            {
                builder.Append(value.ToString("x2"));
            }

            return builder.ToString();
        }
        #endregion
    }
}
