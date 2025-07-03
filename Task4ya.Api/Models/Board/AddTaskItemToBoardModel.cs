using System.ComponentModel.DataAnnotations;
using Task4ya.Application.Board.Commands.Actions;

namespace Task4ya.Api.Models.Board;

public class AddTaskItemToBoardModel
{
	[Required(ErrorMessage = "Board ID is required.")]
	public int BoardId { get; set; }
	[Required(ErrorMessage = "Task Item ID is required.")]
	public int TaskItemId { get; set; }
	
	public AddTaskItemToBoardCommand GetCommand()
	{
		return new AddTaskItemToBoardCommand(BoardId, TaskItemId);
	}
}