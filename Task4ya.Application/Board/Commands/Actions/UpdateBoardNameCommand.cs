using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.Board.Commands.Actions;

public record UpdateBoardNameCommand(int BoardId, string NewName) : IRequest<BoardDto>;