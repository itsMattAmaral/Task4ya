using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.TaskItem.Queries;

public class GetAllTaskItemsQuery : IRequest<IEnumerable<TaskItemDto>>
{
	
}