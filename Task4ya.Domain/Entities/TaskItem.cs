using Task4ya.Domain.Enums;
using Task4ya.Domain.Utils;
namespace Task4ya.Domain.Entities;

public class TaskItem
{
	public int Id { get; init; }
	public int? BoardId { get; set; }
	public string Title { get; set; }
	public string? Description { get; set; }
	public int? AssigneeToId { get; set; }
	public DateTime CreatedAt { get; init; }
	public DateTime UpdatedAt { get; set; }
	public DateTime? DueDate { get; set; }
	public TaskItemStatus Status { get; set; }
	public TaskItemPriority Priority { get; set; }

	public TaskItem(int id, int? boardId, string title, string? description = null, DateTime? dueDate = null, TaskItemPriority priority = TaskItemPriority.Medium, TaskItemStatus status = TaskItemStatus.Pending, int? assigneeToId = null)
	{
		StringValidator.ThrowIfNullOrWhiteSpace(title, "Title");
		Id = id;
		BoardId = boardId;
		Title = title;
		Description = description;
		DueDate = dueDate;
		Status = status;
		Priority = priority;
		CreatedAt = DateTime.UtcNow;
		UpdatedAt = DateTime.UtcNow;
		AssigneeToId = assigneeToId;
	}
	
	public void UpdateTaskItem(int? newBoardId, string? newTitle = null, string? newDescription = null, DateTime? newDueDate = null, TaskItemPriority? newPriority = null, TaskItemStatus? newStatus = null, int? newAssigneeToId = null)
	{
		if (newTitle is not null) UpdateTaskTitle(newTitle);
		if (newDescription is not null) UpdateTaskDescription(newDescription);
		if (newDueDate is not null) UpdateDueDate(newDueDate);
		if (newPriority is not null) UpdatePriority(newPriority.Value);
		if (newStatus is not null) UpdateStatus(newStatus.Value);
		if (newBoardId is not null) UpdateBoardId(newBoardId.Value);
		if (newAssigneeToId is not null) UpdateAssigneeToId(newAssigneeToId.Value);
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
	
	public void UpdateAssigneeToId(int newAssigneeToId)
	{
		AssigneeToId = newAssigneeToId;
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void UpdateBoardId(int? newBoardId)
	{
		BoardId = newBoardId;
		UpdatedAt = DateTime.UtcNow;
	}

	private void UpdateTaskTitle(string newTitle)
	{
		Title = newTitle;
		UpdatedAt = DateTime.UtcNow;
	}

	private void UpdateTaskDescription(string? newDescription)
	{
		Description = newDescription;
		UpdatedAt = DateTime.UtcNow;
	}
}