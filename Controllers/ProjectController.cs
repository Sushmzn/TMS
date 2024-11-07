using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Dto.Projects;
using TaskManagementSystem.EFCore;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController(AppDbContext dbContext) : ControllerBase
{
    private readonly AppDbContext _dbContext = dbContext;

    [HttpPost("api/create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateProjectAsync(ProjectCreateRequest input)
    {
        var newProject = new Project
        {
            Id = new Guid(),
            Title = input.Title,
            Description = input.Description,
        };
        _dbContext.Projects.Add(newProject);
        await _dbContext.SaveChangesAsync();

        return Ok(new { message = "Task created successfully", taskId = newProject.Id });
    }

    [HttpGet("api/get")]
    public async Task<IActionResult> GetAllProjects()
    {
        var projects = await _dbContext.Projects.Select(x => new GetProjectResponse
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description
        }).ToListAsync();

        return Ok(projects);
    }
}