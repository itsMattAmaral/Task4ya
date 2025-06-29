using Task4ya.Domain.Enums;
using Task4ya.Domain.Utils;
namespace Task4ya.Domain.Entities;

public class TaskItem
{
	public int Id { get; init; }
	public string Title { get; set; }
	public string? Description { get; set; }
	public DateTime CreatedAt { get; init; }
	public DateTime UpdatedAt { get; set; }
	public DateTime? DueDate { get; set; }
	public TaskItemStatus Status { get; set; }
	public TaskItemPriority Priority { get; set; }

	public TaskItem(string title, string? description = null, DateTime? dueDate = null, TaskItemPriority priority = TaskItemPriority.Medium, TaskItemStatus status = TaskItemStatus.Pending)
	{
		StringValidator.ThrowIfNullOrWhiteSpace(title, "Title");
		Title = title;
		Description = description;
		DueDate = dueDate;
		Status = status;
		Priority = priority;
		CreatedAt = DateTime.UtcNow;
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void UpdateTaskItem(string? newTitle = null, string? newDescription = null, DateTime? newDueDate = null, TaskItemPriority? newPriority = null, TaskItemStatus? newStatus = null)
	{
		if (newTitle is not null) UpdateTaskTitle(newTitle);
		if (newDescription is not null) UpdateTaskDescription(newDescription);
		if (newDueDate is not null) UpdateDueDate(newDueDate);
		if (newPriority is not null) UpdatePriority(newPriority.Value);
		if (newStatus is not null) UpdateStatus(newStatus.Value);
	}

	public void UpdateTaskTitle(string newTitle)
	{
		Title = newTitle;
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void UpdateTaskDescription(string? newDescription)
	{
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
		var validStatuses = Enum.GetValues<TaskItemStatus>();
		if (!validStatuses.Contains(newStatus))
		{
			throw new ArgumentException($"Invalid status: {newStatus}. Valid statuses are: {string.Join(", ", validStatuses)}");
		}
		Status = newStatus;
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void UpdatePriority(TaskItemPriority newPriority)
	{
		var validPriorities = Enum.GetValues<TaskItemPriority>();
		if (!validPriorities.Contains(newPriority))
		{
			throw new ArgumentException($"Invalid priority: {newPriority}. Valid priorities are: {string.Join(", ", validPriorities)}");
		}
		Priority = newPriority;
		UpdatedAt = DateTime.UtcNow;
	}
	
}