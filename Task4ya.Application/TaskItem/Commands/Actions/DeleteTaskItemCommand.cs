using MediatR;

namespace Task4ya.Application.TaskItem.Commands.Actions;

public record DeleteTaskItemCommand(int Id) : IRequest;