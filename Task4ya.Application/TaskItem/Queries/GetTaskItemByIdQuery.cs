using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.TaskItem.Queries;

public class GetTaskItemByIdQuery(int id) : IRequest<TaskItemDto>
{
	public int Id { get; } = id;
}