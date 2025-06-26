using MediatR;

namespace Task4ya.Application.TaskItem.Commands.Actions;

public class DeleteTaskItemCommand : IRequest
{
	public int Id { get; set; }
	
	public DeleteTaskItemCommand(int id)
	{
		Id = id;
	}
	
}