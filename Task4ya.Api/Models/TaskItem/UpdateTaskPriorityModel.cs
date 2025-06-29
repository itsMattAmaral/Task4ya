using System.ComponentModel.DataAnnotations;
using Task4ya.Application.TaskItem.Commands.Actions;
using Task4ya.Domain.Enums;

namespace Task4ya.Api.Models.TaskItem;

public class UpdateTaskPriorityModel
{
	[Range(0, 3)]
	public TaskItemPriority Priority { get; set; }
	public UpdateTaskPriorityCommand GetCommand(int id)
	{
		return new UpdateTaskPriorityCommand(id, Priority);
	}
}