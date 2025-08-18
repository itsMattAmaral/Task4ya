using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.Board.Commands.Actions;

public record AddBoardCommand(List<int> TaskItemIds, string Name = "New Board") : IRequest<BoardDto>;