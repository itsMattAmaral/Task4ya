using MediatR;
using Task4ya.Application.Dtos;
using Task4ya.Domain.Enums;

namespace Task4ya.Application.TaskItem.Commands.Actions;

public record UpdateTaskItemCommand(
	int Id,
	int? BoardId,
	string? Title = null,
	string? Description = null,
	DateTime? DueDate = null,
	TaskItemPriority Priority = TaskItemPriority.Medium,
	TaskItemStatus Status = TaskItemStatus.Pending,
	int? AssigneeToId = null)
	: IRequest<TaskItemDto>
{
	public int Id { get; set; } = Id;
}