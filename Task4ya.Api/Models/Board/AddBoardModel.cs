using System.ComponentModel.DataAnnotations;
using Task4ya.Application.Board.Commands.Actions;

namespace Task4ya.Api.Models.Board;

public class AddBoardModel
{
	[Display(Name = "Board Name")]
	[MaxLength(100, ErrorMessage = "Board name cannot exceed 100 characters.")]
	[MinLength(3, ErrorMessage = "Board name must be at least 3 characters long.")]
	[RegularExpression(@"^[a-zA-Z0-9\s']+$", ErrorMessage = "Board Name can only contain letters, numbers, spaces and apostrophes.")]
	public string Name { get; set; } = "New Board";
	[Display(Name = "Task Item IDs")]
	public List<int> TaskItemIds { get; set; } = [];
	[Required(ErrorMessage = "OwnerId is required.")]
	[Range(1, int.MaxValue, ErrorMessage = "OwnerId must be a positive integer.")]
	[Display(Name = "Owner ID")]
	public int OwnerId { get; set; }
	
	public AddBoardCommand GetCommand()
	{
		return new AddBoardCommand(OwnerId, TaskItemIds, Name);
	}
}