using System;
using System.Security.Cryptography;
using System.Text;

namespace LoginSystem_24590603
{
    /// <summary>
    /// Hashes passwords with SHA-256 so the real password is never stored
    /// or compared in plain text. This is a one-way function: we hash the
    /// password typed at registration, store only the hash, and at login
    /// time we hash whatever was typed and compare hash-to-hash.
    /// </summary>
    public static class PasswordHasher
    {
        public static string Hash(string plainTextPassword)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(plainTextPassword);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); // lowercase hex
                }
                return sb.ToString();
            }
        }

        public static bool Verify(string plainTextPassword, string storedHash)
        {
            string computedHash = Hash(plainTextPassword);
            return string.Equals(computedHash, storedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
