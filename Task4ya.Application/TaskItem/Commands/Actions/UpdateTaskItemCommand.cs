using MediatR;
using Task4ya.Application.Dtos;
using Task4ya.Domain.Enums;

namespace Task4ya.Application.TaskItem.Commands.Actions;

public record UpdateTaskItemCommand : IRequest<TaskItemDto>
{
	public int Id { get; set; }
	public int BoardId { get; init; }
	public string? Title { get; }
	public string? Description { get; }
	public DateTime? DueDate { get; }
	
	public TaskItemStatus Status { get; init; }
	public TaskItemPriority Priority { get; init; }

	public UpdateTaskItemCommand(
		int id,
		int boardId,
		string? title = null,
		string? description = null,
		DateTime? dueDate = null,
		TaskItemPriority priority = TaskItemPriority.Medium,
		TaskItemStatus status = TaskItemStatus.Pending
		)
	{
		Id = id;
		BoardId = boardId;
		Title = title;
		Description = description;
		DueDate = dueDate;
		Priority = priority;
		Status = status;
	}
	
}