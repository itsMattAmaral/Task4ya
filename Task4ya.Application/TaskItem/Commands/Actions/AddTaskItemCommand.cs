using MediatR;
using Task4ya.Application.Dtos;
using Task4ya.Domain.Enums;

namespace Task4ya.Application.TaskItem.Commands.Actions;

public record AddTaskItemCommand(
	int BoardId,
	string Title,
	string? Description = null,
	DateTime? DueDate = null,
	TaskItemPriority Priority = TaskItemPriority.Medium,
	TaskItemStatus Status = TaskItemStatus.Pending,
	int? AssigneeToId = null)
	: IRequest<TaskItemDto>;