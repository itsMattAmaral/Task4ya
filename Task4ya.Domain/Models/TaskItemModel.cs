using Task4ya.Domain.Enums;

namespace Task4ya.Domain.Models;

public class TaskItemModel
{
	public int Id { get; set; }
	public required string Title { get; set; }
	public string? Description { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
	public DateTime? DueDate { get; set; }
	public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
	public TaskItemPriority Priority { get; set; } = TaskItemPriority.Medium;
}