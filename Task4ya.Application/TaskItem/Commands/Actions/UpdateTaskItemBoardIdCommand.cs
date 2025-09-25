using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.TaskItem.Commands.Actions;

public record UpdateTaskItemBoardIdCommand(int NewBoardId ) : IRequest<TaskItemDto>
{
	public int Id {get; set;}
}