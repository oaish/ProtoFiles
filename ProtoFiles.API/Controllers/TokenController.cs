using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ProtoFiles.Lib.Dto;
using ProtoFiles.Lib.Models;

namespace ProtoFiles.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TokenController(IConfiguration config) : ControllerBase
{
    [HttpPost("[action]")]
    public ActionResult<string> Generate([FromBody] JwtPayload payload)
    {
        var tokenHandler = new JsonWebTokenHandler();
        var key = Encoding.ASCII.GetBytes(config["JwtSettings:Key"]!);

        var claims = new List<Claim>()
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, payload.Username),
            new(JwtRegisteredClaimNames.Email, payload.Username),
            new("userid", payload.UserId.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            IssuedAt = DateTime.UtcNow,
            Issuer = config["JwtSettings:Issuer"]!,
            Audience = config["JwtSettings:Audience"]!,
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(token);
    }
}