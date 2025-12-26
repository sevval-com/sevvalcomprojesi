using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Utilities
{
    public static class ReferenceHelper
    {
        public static string GenerateReferenceCode()
        {
            var timeBytes = BitConverter.GetBytes(DateTime.UtcNow.Ticks);
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(timeBytes);
                return Convert.ToBase64String(hash)
                    .Replace("/", "").Replace("+", "").Substring(0, 10); // Example: "A1B2C3D4E5"
            }
        }

        public static string NormalizeUsername(string input)
        {
            input = input.Replace("ç", "c")
                    .Replace("ğ", "g")
                    .Replace("ı", "i")
                    .Replace("ö", "o")
                    .Replace("ş", "s")
                    .Replace("ü", "u");
            return input;
        }
        public static string NormalizeCompanyName(string companyName)
        {
            if (string.IsNullOrEmpty(companyName)) return string.Empty;

            var normalized = companyName.Replace(" ", "").ToLowerInvariant();

            normalized = normalized
                .Replace("ı", "i")
                .Replace("ğ", "g")
                .Replace("ü", "u")
                .Replace("ş", "s")
                .Replace("ö", "o")
                .Replace("ç", "c")
                .Replace("İ", "i")
                .Replace("Ğ", "g")
                .Replace("Ü", "u")
                .Replace("Ş", "s")
                .Replace("Ö", "o")
                .Replace("Ç", "c");

            return normalized;
        }


    }
}
