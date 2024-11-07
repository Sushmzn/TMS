using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models;

public class Task
{
    [Key] public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime CompletionDate { get; set; }
}