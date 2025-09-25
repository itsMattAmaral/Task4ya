using Task4ya.Application.TaskItem.Commands.Actions;

namespace Task4ya.Api.Models.TaskItem;

public class UpdateTaskItemBoardIdModel
{
	public int NewBoardId { get; set; }
	
	public UpdateTaskItemBoardIdCommand GetCommand()
	{
		return new UpdateTaskItemBoardIdCommand(NewBoardId);
	}
}