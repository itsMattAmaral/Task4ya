using System.ComponentModel.DataAnnotations;
using Task4ya.Application.Board.Commands.Actions;

namespace Task4ya.Api.Models.Board;

public class AddTaskItemToBoardModel
{
	[Required(ErrorMessage = "Task Item ID is required.")]
	public int TaskItemId { get; set; }
	
	public AddTaskItemToBoardCommand GetCommand(int boardId)
	{
		return new AddTaskItemToBoardCommand(boardId, TaskItemId);
	}
}