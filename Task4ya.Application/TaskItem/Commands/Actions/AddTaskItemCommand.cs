using MediatR;
using Task4ya.Application.Dtos;
using Task4ya.Domain.Enums;

namespace Task4ya.Application.TaskItem.Commands.Actions;

public class AddTaskItemCommand : IRequest<TaskItemDto>
{
	public string Title { get; }
	public string? Description { get; }
	public DateTime? DueDate { get; }
	
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