using Task4ya.Application.TaskItem.Commands.Actions;
using Task4ya.Domain.Enums;

namespace Task4ya.Api.Models.TaskItem;

public class UpdateTaskItemModel
{
	public string? Title { get; set; }
	public string? Description { get; set; }
	public DateTime? DueDate { get; set; }
	public TaskItemPriority Priority { get; set; }
	public TaskItemStatus Status { get; set; }

	public UpdateTaskItemCommand GetCommand(int id)
	{
		return new UpdateTaskItemCommand(id, Title, Description, DueDate, Priority, Status);
	}
}