using MediatR;

namespace Task4ya.Application.Board.Commands.Actions;

public record AddTaskItemToBoardCommand(int BoardId, int TaskItemId) : IRequest;