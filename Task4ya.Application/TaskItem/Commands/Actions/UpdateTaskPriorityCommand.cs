using MediatR;
using Task4ya.Application.Dtos;
using Task4ya.Domain.Enums;

namespace Task4ya.Application.TaskItem.Commands.Actions;

public class UpdateTaskPriorityCommand : IRequest<TaskItemDto>
{
	public int Id { get; set; }
	public TaskItemPriority Priority { get; set; }
	public UpdateTaskPriorityCommand(int id, TaskItemPriority priority)
	{
		Id = id;
		Priority = priority;
	}
}