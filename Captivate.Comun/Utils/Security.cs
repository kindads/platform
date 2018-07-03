using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Common.Utils
{
    public static class Security
    {
        public static string GetSha256(string siteToken)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(siteToken));
            string salt = ConfigurationManager.AppSettings["Salt"];

            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString(salt));
            }
            return hash.ToString();
        }
    }
}
