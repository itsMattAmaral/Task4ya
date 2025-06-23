using Task4ya.Domain.Enums;

namespace Task4ya.Application.Dtos;

public class TaskItemDto
{
	public int Id { get; init; }
	public required string Title { get; init; }
	public string? Description { get; init; }
	public DateTime? DueDate { get; init; }
	public TaskItemStatus Status { get; set; }
	public TaskItemPriority Priority { get; set; }
}