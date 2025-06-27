using System.ComponentModel.DataAnnotations;
using Task4ya.Application.Board.Commands.Actions;

namespace Task4ya.Api.Models.Board;

public class AddBoardModel
{
	[Display(Name = "Board Name")]
	[MaxLength(100, ErrorMessage = "Board name cannot exceed 100 characters.")]
	[MinLength(3, ErrorMessage = "Board name must be at least 3 characters long.")]
	public string Name { get; set; } = "New Board";
	public List<int> TaskItemIds { get; set; } = [];
	
	public AddBoardCommand GetCommand()
	{
		return new AddBoardCommand(TaskItemIds, Name);
	}
}