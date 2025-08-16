using Task4ya.Domain.Enums;

namespace Task4ya.Application.Dtos;

public class TaskItemDto
{
	public int Id { get; init; }
	public int BoardId { get; init; }
	public int? AssigneeToId { get; init; }
	public required string Title { get; init; }
	public string? Description { get; init; }
	public DateTime? DueDate { get; init; }
	
	public DateTime CreatedAt { get; init; }
	
	public DateTime UpdatedAt { get; init; }
	public TaskItemStatus Status { get; init; }
	public TaskItemPriority Priority { get; init; }
}