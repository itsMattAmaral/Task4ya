using MediatR;

namespace Task4ya.Application.Board.Commands.Actions;

public class RemoveTaskItemToBoardCommand : IRequest
{
	public int BoardId { get; }
	public int TaskItemId { get; }

	public RemoveTaskItemToBoardCommand(int boardId, int taskItemId)
	{
		BoardId = boardId;
		TaskItemId = taskItemId;
	}
	
}