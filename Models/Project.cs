using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models;

public class Project
{
    [Key] public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}