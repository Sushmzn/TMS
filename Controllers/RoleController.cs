using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.EFCore;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController(AppDbContext dbContext) : ControllerBase
{
    private readonly AppDbContext _dbContext = dbContext;

    // Explicitly define this method as an HTTP POST request
    [HttpPost("create")]
    public async Task<IActionResult> CreateRoleAsync(string roleName)
    {
        var role = new Role
        {
            Id = new Guid(),
            Name = roleName
        };
        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync();
        return Ok(role.Id);
    }
}