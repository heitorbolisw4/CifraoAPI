using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackEnd.Domain.Entities;
using BackEnd.Domain.Interface;
using BackEnd.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BackEnd.Service;


public class TokenService(IConfiguration config, IOptions<JwtSettings> jwtOptions) : ITokenService
{
    private readonly IConfiguration _config = config; 
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public string GenerateToken(User user)
    {
        var secretKey = _jwtSettings.SecretKey;
        var audience = _jwtSettings.Audience;
        var issuer = _jwtSettings.Issuer;
        var expiration = _jwtSettings.ExpirationInMinutes;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Name)
        };
        var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiration),
                signingCredentials: creds
            
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}