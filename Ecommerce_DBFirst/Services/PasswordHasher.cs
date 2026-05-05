using System.Security.Cryptography;
using System.Text;

namespace Ecommerce_DBFirst.Services
{
    public static class PasswordHasher
    {
        public static bool VerifyPassword(string password, string saltBase64, string expectedHashBase64)
        {
            if (string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(saltBase64) ||
                string.IsNullOrWhiteSpace(expectedHashBase64))
            {
                return false;
            }

            byte[] salt;
            byte[] expectedHash;

            try
            {
                salt = Convert.FromBase64String(saltBase64);
                expectedHash = Convert.FromBase64String(expectedHashBase64);
            }
            catch (FormatException)
            {
                return false;
            }

            using var pbkdf2 = new Rfc2898DeriveBytes(
                Encoding.UTF8.GetBytes(password),
                salt,
                100_000,
                HashAlgorithmName.SHA256);

            var computedHash = pbkdf2.GetBytes(expectedHash.Length);
            return CryptographicOperations.FixedTimeEquals(computedHash, expectedHash);
        }
    }
}
