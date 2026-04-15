using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Equillibrium.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // The method MUST be inside these braces
    [HttpGet("dev-token/{tenantId}")]
    [AllowAnonymous]
    public IActionResult GetDevToken(Guid tenantId)
    {
        var claims = new[] {
            new Claim("TenantId", tenantId.ToString()),
            new Claim(ClaimTypes.Name, "DevUser")
        };

        // Note: This key must match what's in your Program.cs/appsettings.json
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YOUR_VERY_LONG_SECRET_KEY_HERE_32_CHARS"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "EquillibriumAPI",
            audience: "EquillibriumUsers",
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
}
