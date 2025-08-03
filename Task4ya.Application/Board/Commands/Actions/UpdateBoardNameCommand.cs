using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.Board.Commands.Actions;

public class UpdateBoardNameCommand : IRequest<BoardDto>
{
	public int BoardId { get; }
	public string NewName { get; }
	
	public UpdateBoardNameCommand(int boardId, string newName)
	{
		BoardId = boardId;
		NewName = newName;
	}
}