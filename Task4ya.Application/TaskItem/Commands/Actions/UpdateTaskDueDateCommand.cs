using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.TaskItem.Commands.Actions;

public record UpdateTaskDueDateCommand(int Id, DateTime? DueDate) : IRequest<TaskItemDto>;