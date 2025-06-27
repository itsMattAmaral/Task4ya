using System.ComponentModel.DataAnnotations;
using Task4ya.Application.TaskItem.Commands.Actions;
using Task4ya.Domain.Enums;

namespace Task4ya.Api.Models.TaskItem;

public class UpdateTaskStatusModel
{
	[Range(0, 3)]
	public TaskItemStatus Status { get; set; }

	public UpdateTaskStatusCommand GetCommand(int id)
	{
		return new UpdateTaskStatusCommand(id, Status);
	}
}