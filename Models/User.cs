using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Guid RoleId { get; set; }
}