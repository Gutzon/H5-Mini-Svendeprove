using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RegnskabsSystem.Helpers
{
    public static class SecurityHelper
    {
        public static string GetHashCode(string textToHash)
        {
            var salt = "SaltetMadSmagerAfMere";
            using SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(textToHash + salt));
            return String.Join("", bytes.Select(b => b.ToString("x2")));
        }
    }
}
