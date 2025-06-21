using MediatR;
using Microsoft.EntityFrameworkCore;
using Task4ya.Application.Dtos;
using Task4ya.Infrastructure.Data;

namespace Task4ya.Application.TaskItem.Queries;

public class TaskItemsQueryHandler : IRequestHandler<GetAllTaskItemsQuery, IEnumerable<TaskItemDto>>
{
	private readonly Task4YaDbContext _dbcontext;

	public TaskItemsQueryHandler(Task4YaDbContext dbcontext)
	{
		_dbcontext = dbcontext;
	}

	public async Task<IEnumerable<TaskItemDto>> Handle(GetAllTaskItemsQuery request, CancellationToken cancellationToken)
	{
		var tasks = await _dbcontext.TaskItems.ToListAsync(cancellationToken);
		return tasks.Select(task => new TaskItemDto
		{
			Id = task.Id,
			Title = task.Title,
			Description = task.Description,
			DueDate = task.DueDate,
			Status = task.Status,
			Priority = task.Priority,
			CreatedAt = task.CreatedAt,
			UpdatedAt = task.UpdatedAt
		});
	}
	
	public async Task<TaskItemDto> Handle(GetTaskItemByIdQuery request, CancellationToken cancellationToken)
	{
		var task = await _dbcontext.TaskItems
			.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

		if (task == null)
		{
			throw new KeyNotFoundException($"Task with ID {request.Id} not found.");
		}

		return new TaskItemDto
		{
			Id = task.Id,
			Title = task.Title,
			Description = task.Description,
			DueDate = task.DueDate,
			Status = task.Status,
			Priority = task.Priority,
			CreatedAt = task.CreatedAt,
			UpdatedAt = task.UpdatedAt
		};
	}
}