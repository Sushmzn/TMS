namespace TaskManagementSystem.Dto;

public class TaskCreateRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime CompletionDate { get; set; }
}
