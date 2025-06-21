using Task4ya.Domain.Enums;
using Task4ya.Domain.Utils;
namespace Task4ya.Domain.Entities;

public class TaskItem
{
	public int Id { get; set; }
	public string Title { get; set; }
	public string? Description { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
	public DateTime? DueDate { get; set; }
	public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
	public TaskItemPriority Priority { get; set; } = TaskItemPriority.Medium;

	public TaskItem(string title, string? description = null, DateTime? dueDate = null)
	{
		StringValidator.ThrowIfNullOrWhiteSpace(title, "Title");
		StringValidator.ThrowIfNullOrWhiteSpace(description, "Description");
		Title = title;
		Description = description;
		DueDate = dueDate;
	}
	
	public void UpdateTaskTitle(string newTitle)
	{
		StringValidator.ThrowIfNullOrWhiteSpace(newTitle, "Title");
		Title = newTitle;
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void UpdateTaskDescription(string? newDescription)
	{
		StringValidator.ThrowIfNullOrWhiteSpace(newDescription, "Description");
		Description = newDescription;
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void UpdateDueDate(DateTime? newDueDate)
	{
		DueDate = newDueDate;
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void UpdateStatus(TaskItemStatus newStatus)
	{
		Status = newStatus;
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void UpdatePriority(TaskItemPriority newPriority)
	{
		Priority = newPriority;
		UpdatedAt = DateTime.UtcNow;
	}
	
}