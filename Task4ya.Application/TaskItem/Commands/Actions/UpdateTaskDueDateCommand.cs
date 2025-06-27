using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.TaskItem.Commands.Actions;

public class UpdateTaskDueDateCommand : IRequest<TaskItemDto>
{
	public int Id { get; set; }
	public DateTime? DueDate { get; set; }
	public UpdateTaskDueDateCommand(int id, DateTime? dueDate)
	{
		Id = id;
		DueDate = dueDate;
	}
}