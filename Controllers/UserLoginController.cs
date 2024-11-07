using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TaskManagementSystem.EFCore;
using LoginRequest = TaskManagementSystem.Dto.User.LoginRequest;

namespace TaskManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserLoginController(AppDbContext dbContext, IConfiguration configuration) : ControllerBase
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IConfiguration _configuration = configuration;

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest loginRequest)
    {
        var user = _dbContext.Users.FirstOrDefault(user => user.Name == loginRequest.Username);
        var role = _dbContext.Roles.FirstOrDefault(role => role.Id == user.RoleId);
        // Validate credentials (in real applications, validate against database)
        if (loginRequest.Username != user.Name || loginRequest.Password != user.Password) return Unauthorized();
        // Create the token
        var token = GenerateJwtToken(user.Name, role.Name);
        return Ok(new { token });
    }

    private string GenerateJwtToken(string username, string role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        const string secretKey = "SftYUnIdWFAqlFdPxPHnxkQLhlZNBLxb";
        var key = Encoding.ASCII.GetBytes(secretKey);
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, username), // Add claims as needed
            new Claim(ClaimTypes.Role, role)
        };
        var skey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(skey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "https://localhost:44369/",
            audience: "https://localhost:44369/",
            claims: claims,
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddDays(20), // Expiration time in the future
            signingCredentials: credentials // Signing credentials must be valid
        );
        var tokenData = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenData;

        // var tokenDescriptor = new SecurityTokenDescriptor
        // {
        //     Subject = new ClaimsIdentity(new[]
        //     {
        //         new Claim(ClaimTypes.Name, username), // Add claims as needed
        //         new Claim(ClaimTypes.Role, role)
        //     }),
        //     Expires = DateTime.UtcNow.AddDays(10), // Token expiry
        //     SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        // };
        //
        // var token = tokenHandler.CreateToken(tokenDescriptor);
        // return tokenHandler.WriteToken(token);
    }
}