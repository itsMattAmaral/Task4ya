using Task4ya.Application.TaskItem.Commands.Actions;

namespace Task4ya.Api.Models.TaskItem;

public class UpdateTaskDueDateModel
{
	public DateTime? DueDate { get; set; }
	public UpdateTaskDueDateCommand GetCommand(int id)
	{
		return new UpdateTaskDueDateCommand(id, DueDate);
	}
}