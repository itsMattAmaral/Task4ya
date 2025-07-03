using MediatR;

namespace Task4ya.Application.Board.Commands.Actions;

public class DeleteBoardCommand : IRequest
{
	public int Id { get; }
	
	public DeleteBoardCommand(int id)
	{
		Id = id;
	}
}