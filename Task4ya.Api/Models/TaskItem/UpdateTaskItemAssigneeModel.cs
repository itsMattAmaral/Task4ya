using Task4ya.Application.TaskItem.Commands.Actions;

namespace Task4ya.Api.Models.TaskItem;

public class UpdateTaskItemAssigneeModel
{
	public int NewAssigneeId { get; set; }
	
	public UpdateTaskItemAssigneeToIdCommand GetCommand()
	{
		return new UpdateTaskItemAssigneeToIdCommand(NewAssigneeId);
	}
}