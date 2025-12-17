using System;
using System.Security.Cryptography;
using System.Text;

namespace ParkingApp.Utils
{
    /// <summary>
    /// Security Helper - Password hashing và security utilities
    /// </summary>
    public static class SecurityHelper
    {
        /// <summary>
        /// Hash password using SHA256 (simplified version - in production use BCrypt)
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty");

            using (SHA256 sha256 = SHA256.Create())
            {
                // Add salt for better security
                string saltedPassword = password + "ParkingAppV3_SecretSalt_2025";
                
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                
                return builder.ToString();
            }
        }

        /// <summary>
        /// Verify if password matches the hash
        /// </summary>
        public static bool VerifyPassword(string password, string hash)
        {
            string hashedPassword = HashPassword(password);
            return string.Equals(hashedPassword, hash, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Generate a secure random token (for sessions, etc.)
        /// </summary>
        public static string GenerateSecureToken(int length = 32)
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[length];
                rng.GetBytes(tokenData);

                StringBuilder token = new StringBuilder();
                foreach (byte b in tokenData)
                {
                    token.Append(b.ToString("x2"));
                }

                return token.ToString();
            }
        }

        /// <summary>
        /// Check if password meets minimum requirements
        /// </summary>
        public static bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            // Minimum 6 characters for now (can be enhanced)
            return password.Length >= 6;
        }

        /// <summary>
        /// Sanitize input to prevent SQL injection (basic check)
        /// </summary>
        public static string SanitizeInput(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Remove dangerous characters
            string sanitized = input.Replace("'", "''")
                                   .Replace("--", "")
                                   .Replace(";", "")
                                   .Replace("/*", "")
                                   .Replace("*/", "")
                                   .Replace("xp_", "")
                                   .Replace("sp_", "");

            return sanitized;
        }
    }
}
