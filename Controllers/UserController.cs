using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Dto.User;
using TaskManagementSystem.EFCore;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers;

public class UserController(AppDbContext dbContext) : ControllerBase
{
    private readonly AppDbContext _dbContext = dbContext;

    // Explicitly define this method as an HTTP POST request
    [HttpPost("create")]
    public async Task<IActionResult> CreateUserAsync(UserCreateRequest input)
    {
        var user = new User
        {
            Id = new Guid(),
            Name = input.Name,
            Email = input.Email,
            Password = input.Password,
            RoleId = input.RoleId
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return Ok(user.Id);
    }
}