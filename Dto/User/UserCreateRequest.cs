namespace TaskManagementSystem.Dto.User;

public class UserCreateRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Guid RoleId { get; set; }
}