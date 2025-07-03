using MediatR;

namespace Task4ya.Application.Board.Commands.Actions;

public class AddTaskItemToBoardCommand : IRequest
{
	public int BoardId { get; }
	public int TaskItemId { get; }
	
	public AddTaskItemToBoardCommand(int boardId, int taskItemId)
	{
		BoardId = boardId;
		TaskItemId = taskItemId;
	}
}