using FinCtrlLibrary.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FinCtrlApi.Services
{
    public static class TokenService
    {
        private static string _secret;
        private static int _hoursToExpire;

        public static void SetSecret(string secret)
        {
            _secret = secret;
        }

        public static void SetHoursToExpire(int hoursToExpire)
        {
            _hoursToExpire = hoursToExpire;
        }

        public static string GenerateToken(User user)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_secret);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new(ClaimTypes.Name, user.Name),
                    new(ClaimTypes.Sid, user._id.ToString()),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.DateOfBirth, user.DateOfBirth.ToShortDateString()),
                    new(ClaimTypes.Role, user.Role)
                ]),
                Expires = DateTime.UtcNow.AddHours(_hoursToExpire),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
