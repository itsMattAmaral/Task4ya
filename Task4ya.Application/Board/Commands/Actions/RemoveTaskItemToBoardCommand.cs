using MediatR;

namespace Task4ya.Application.Board.Commands.Actions;

public record RemoveTaskItemToBoardCommand(int BoardId, int TaskItemId) : IRequest;