using MediatR;
using Microsoft.EntityFrameworkCore;
using Task4ya.Application.Dtos;
using Task4ya.Application.Mappers;
using Task4ya.Infrastructure.Data;

namespace Task4ya.Application.TaskItem.Queries;

public class TaskItemsQueryHandler : IRequestHandler<GetAllTaskItemsQuery, IEnumerable<TaskItemDto>>, IRequestHandler<GetTaskItemByIdQuery, TaskItemDto>
{
	private readonly Task4YaDbContext _dbcontext;

	public TaskItemsQueryHandler(Task4YaDbContext dbcontext)
	{
		_dbcontext = dbcontext;
	}

	public async Task<IEnumerable<TaskItemDto>> Handle(GetAllTaskItemsQuery request, CancellationToken cancellationToken)
	{
		var tasks = await _dbcontext.TaskItems.ToListAsync(cancellationToken);
		return tasks.Select(taskItem => taskItem.MapToDto());
	}
	
	public async Task<TaskItemDto> Handle(GetTaskItemByIdQuery request, CancellationToken cancellationToken)
	{
		var taskItem = await _dbcontext.TaskItems
			.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

		if (taskItem == null)
		{
			throw new KeyNotFoundException($"Task with ID {request.Id} not found.");
		}

		return taskItem.MapToDto();
	}
}