using MediatR;

namespace Task4ya.Application.Board.Commands.Actions;

public record UpdateBoardOwnerCommand(int BoardId, int NewOwnerId) : IRequest;