using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.EFCore;
using Task = TaskManagementSystem.Models.Task;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManagementSystem.Dto;
using System;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagementSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public TaskController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        // Method to create a new task
        [HttpPost("create")]
        public async Task<IActionResult> CreateTaskAsync([FromBody] TaskCreateRequest request)
        {
            var newTask = new Task
            {
                Title = request.Title,
                Description = request.Description,
                ProjectId = request.ProjectId,
                CompletionDate = request.CompletionDate,
                Status = "Pending"
            };

            _appDbContext.Tasks.Add(newTask);
            await _appDbContext.SaveChangesAsync();

            return Ok(new { message = "Task created successfully", taskId = newTask.Id });
        }

        // GET method to fetch all tasks for a specific project
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllTasksAsync(Guid projectId)
        {
            var tasks = await _appDbContext.Tasks
                .Where(x => x.ProjectId == projectId)
                .Select(x => new GetTaskResponse
                {
                    Title = x.Title,
                    Description = x.Description,
                    ProjectId = x.ProjectId,
                    Id = x.Id,
                    Status = x.Status,
                }).ToListAsync();

            return Ok(tasks);
        }       

        // GET method to fetch completed tasks, using pattern matching to filter by completion status
        [HttpGet("get-completed")]
        public async Task<IActionResult> GetCompletedTasksAsync(Guid projectId)
        {
            var tasks = await _appDbContext.Tasks
                .Where(x => x.ProjectId == projectId && x.Status == "Completed")
                .Select(x => new GetTaskResponse
                {
                    Title = x.Title,
                    Description = x.Description,
                    ProjectId = x.ProjectId,
                    Id = x.Id,
                    Status = x.Status,
                }).ToListAsync();

            return Ok(tasks);
        }

        // PUT method to update task status using pattern matching for validation
        [HttpPut("update-status/{taskId}")]
        public async Task<IActionResult> UpdateTaskStatusAsync(Guid taskId, [FromBody] string status)
        {
            var task = await _appDbContext.Tasks.FindAsync(taskId);

            if (task == null)
            {
                return NotFound("Task not found.");
            }

            // Pattern matching to check if the status is valid
            task.Status = status switch
            {
                "Completed" => "Completed",
                "InProgress" => "InProgress",
                "OnHold" => "OnHold",
                _ => throw new ArgumentException("Invalid status provided.")
            };

            await _appDbContext.SaveChangesAsync();
            return Ok(new { message = "Task status updated successfully." });
        }

        // DELETE method to remove a task, with pattern matching for confirmation
        [HttpDelete("delete/{taskId}")]
        public async Task<IActionResult> DeleteTaskAsync(Guid taskId)
        {
            var task = await _appDbContext.Tasks.FindAsync(taskId);

            if (task == null)
            {
                return NotFound("Task not found.");
            }

            // Using pattern matching to check the status of the task before deletion
            if (task.Status is "Completed")
            {
                return BadRequest("Completed tasks cannot be deleted.");
            }

            _appDbContext.Tasks.Remove(task);
            await _appDbContext.SaveChangesAsync();

            return Ok(new { message = "Task deleted successfully." });
        }

        // GET method to fetch today's tasks, using pattern matching on DateTime
        [HttpGet("get-todays-tasks")]
        public async Task<IActionResult> GetTodayTasksAsync(Guid projectId)
        {
            var tasks = await _appDbContext.Tasks
                .Where(x => x.ProjectId == projectId &&
                            x.CompletionDate == DateTime.UtcNow)
                .Select(x => new GetTaskResponse
                {
                    Title = x.Title,
                    Description = x.Description,
                    ProjectId = x.ProjectId,
                    Id = x.Id,
                    Status = x.Status,
                    CompletionDate = x.CompletionDate
                }).ToListAsync();

            return Ok(tasks);
        }

        // API to retrieve tasks based on their status
        [HttpGet("get-by-status")]
        public async Task<IActionResult> GetTasksByStatusAsync(string status)
        {
            // Pattern matching to validate the status input
            var validStatus =
                status switch
                {
                    "Completed" => "Completed",
                    "Pending" => "Pending",
                    "OnHold" => "OnHold",
                    _ => throw new ArgumentException("Invalid status provided.")
                };
            var tasks = await _appDbContext.Tasks
                .Where(x => x.Status == validStatus)
                .Select(x => new GetTaskResponse
                {
                    Title = x.Title,
                    Description = x.Description,
                    ProjectId = x.ProjectId,
                    Id = x.Id,
                    Status = x.Status,
                    CompletionDate = x.CompletionDate
                }).ToListAsync();

            return Ok(tasks);
        }

        [HttpGet("get-overdue-tasks")]
        public async Task<IActionResult> GetOverdueTasksAsync(Guid projectId)
        {
            var today = DateTime.UtcNow;

            // Fetch overdue tasks from the database
            var tasks = await _appDbContext.Tasks
                .Where(x => x.ProjectId == projectId && x.CompletionDate < today)
                .ToListAsync();

            // Apply the switch expression after retrieving data from the database
            var taskResponses = tasks.Select(x => new
            {
                x.Title,
                x.Description,
                x.ProjectId,
                x.Id,
                Urgency = (today - x.CompletionDate).TotalDays switch
                {
                    > 30 => "Critical",
                    > 7 => "High",
                    _ => "Moderate"
                },
                x.Status,
                x.CompletionDate
            }).ToList();

            return Ok(taskResponses);
        }

        // API to fetch tasks based on their completion date being today, using pattern matching with DateTime
        [HttpGet("get-tasks-due-today")]
        public async Task<IActionResult> GetTasksDueTodayAsync(Guid projectId)
        {
            var today = DateTime.UtcNow.Date; 

            var tasks = await _appDbContext.Tasks
                .Where(x => x.ProjectId == projectId && x.CompletionDate.Date == today)
                .Select(x => new GetTaskResponse
                {
                    Title = x.Title,
                    Description = x.Description,
                    ProjectId = x.ProjectId,
                    Id = x.Id,
                    CompletionDate = x.CompletionDate,
                    Status = x.Status
                }).ToListAsync();

            if (tasks.Any(t => t.Status is not "Completed"))
            {
                // If there are tasks that are not completed, you can filter them separately if needed
                tasks = tasks.Where(t => t.Status is not "Completed").ToList();
            }

            return Ok(tasks);
        }
    }
}