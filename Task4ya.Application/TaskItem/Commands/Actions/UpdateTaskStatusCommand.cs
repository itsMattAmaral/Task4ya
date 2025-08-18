using MediatR;
using Task4ya.Application.Dtos;
using Task4ya.Domain.Enums;

namespace Task4ya.Application.TaskItem.Commands.Actions;

public record UpdateTaskStatusCommand(int Id, TaskItemStatus Status) : IRequest<TaskItemDto>;