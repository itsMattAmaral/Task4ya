using MediatR;

namespace Task4ya.Application.Board.Commands.Actions;

public record DeleteBoardCommand(int Id) : IRequest;