using MediatR;
using Task4ya.Application.Dtos;
using Task4ya.Domain.Enums;

namespace Task4ya.Application.TaskItem.Commands.Actions;

public class UpdateTaskStatusCommand : IRequest<TaskItemDto>
{
	public int Id { get; set; }
	public TaskItemStatus Status { get; set; }
	
	public UpdateTaskStatusCommand(int id, TaskItemStatus status)
	{
		Id = id;
		Status = status;
	}
}