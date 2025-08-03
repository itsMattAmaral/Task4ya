using System.ComponentModel.DataAnnotations;
using Task4ya.Application.Board.Commands.Actions;

namespace Task4ya.Api.Models.Board;

public class RemoveTaskItemToBoardModel
{
	[Required(ErrorMessage = "Board ID is required.")]
	public int BoardId { get; set; }
	[Required(ErrorMessage = "Task Item ID is required.")]
	public int TaskItemId { get; set; }
	
	public RemoveTaskItemToBoardCommand GetCommand()
	{
		return new RemoveTaskItemToBoardCommand(BoardId, TaskItemId);
	}
}