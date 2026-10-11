using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TurnBasedWebApi.Models;

namespace TurnBasedWebApi.Services
{
    public interface ITokenService
    {
        public string GenerateToken(Player player);
    }

    public class TokenService(IConfiguration configuration) : ITokenService
    {
        public string GenerateToken(Player player)
        {
            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(configuration["JWTKEY"]!));
            if (key == null) throw new ArgumentNullException("JWTKEY is not found in secrets or env.");

            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

            Claim[] claims =
            [
                new Claim(ClaimTypes.NameIdentifier, player.Id.ToString()),
                new Claim(ClaimTypes.Name, player.Name),
                new Claim(ClaimTypes.Email, player.Email)
            ];

            JwtSecurityToken token = new(
                issuer: configuration["JWTISSUER"],
                audience: configuration["JWTAUDIENCE"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}