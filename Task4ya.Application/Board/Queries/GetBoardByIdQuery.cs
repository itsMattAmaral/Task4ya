using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.Board.Queries;

public record GetBoardByIdQuery(int Id) : IRequest<BoardDto?>;