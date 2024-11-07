namespace TaskManagementSystem.Dto;

public class GetTaskResponse : TaskCreateRequest
{
    public Guid Id { get; set; }
    public string Status { get; set; }
}