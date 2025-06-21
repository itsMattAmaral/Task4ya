using MediatR;
using Task4ya.Application.Dtos;
using Task4ya.Domain.Enums;

namespace Task4ya.Application.TaskItem.Commands.Actions;

public class AddTaskItemCommand : IRequest<TaskItemDto>
{
	public required string Title { get; init; }
	public string? Description { get; init; }
	public DateTime? DueDate { get; init; }
	
	public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
	
	public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
	
	public TaskItemStatus Status { get; init; }
	public TaskItemPriority Priority { get; init; }

	public AddTaskItemCommand(
		string title,
		string? description = null,
		DateTime? dueDate = null,
		TaskItemPriority priority = TaskItemPriority.Medium,
		TaskItemStatus status = TaskItemStatus.Pending
		)
	{
		Title = title;
		Description = description;
		DueDate = dueDate;
		Priority = priority;
		Status = status;
	}
	
}