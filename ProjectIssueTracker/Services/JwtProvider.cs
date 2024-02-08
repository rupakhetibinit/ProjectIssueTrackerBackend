using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure;
using Microsoft.IdentityModel.Tokens;
using ProjectIssueTracker.Models;

namespace ProjectIssueTracker.Services;

public class JwtProvider(IConfiguration config) : IJwtProvider
{
    public string Generate(User user)
    {
            List<Claim> claims =
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            ];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("AppSettings:Secret").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var expiretime = DateTime.Now.AddDays(7);
            var token = new JwtSecurityToken(claims: claims, expires: expiretime, signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
    }
}