using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.Board.Queries;

public class GetBoardByIdQuery(int id) : IRequest<BoardDto>
{
	public int Id { get; } = id;
}