using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CryptoWalletApp.Models;

namespace CryptoWalletApp.Services
{
    public class TokenGenerator
    {
        private readonly IRefreshTokenRepository dprepo;
        private readonly IConfiguration configuration;
        public TokenGenerator(IRefreshTokenRepository dprepo, IConfiguration configuration) {
            this.dprepo = dprepo;
            this.configuration = configuration;
        }
        public static string GenerateJwtToken(IConfiguration configuration, UserRoleDto urdto)
        {
            if (urdto == null)
            {
                throw new Exception("userroledto cant be null");
            }
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, urdto.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, urdto.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };
            foreach (var role in urdto.RoleIDs)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(0.5), 
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"],
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public static string GenerateRefreshToken()
        {
            var randomBytes = new byte[12]; 
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            var refreshToken = Convert.ToBase64String(randomBytes)
                .TrimEnd('=') 
                .Replace('+', '-') 
                .Replace('/', '_')
                .Substring(0, 16); 
            return refreshToken;
        }
        public string? GenerateAccessTokenFromRefreshToken(string refreshToken)
        {
            UserRoleDto? user;
            if (!dprepo.IsExistRefreshToken(refreshToken))
            {
                user = dprepo.GetUserRoleDtoFromRefreshToken(refreshToken);
                if (user == null)
                    throw new Exception("UserDto to create access token cant be null");
                return GenerateJwtToken(configuration, user);
            }
            return null;
        }
    }
}
    