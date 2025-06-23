using Task4ya.Domain.Enums;

namespace Task4ya.Application.Dtos;

public class TaskItemDto
{
	public int Id { get; set; }
	public required string Title { get; set; }
	public string? Description { get; set; }
	public DateTime? DueDate { get; set; }
	public TaskItemStatus Status { get; set; }
	public TaskItemPriority Priority { get; set; }
}