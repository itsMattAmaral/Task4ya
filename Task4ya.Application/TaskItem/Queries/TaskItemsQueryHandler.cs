using MediatR;
using Microsoft.EntityFrameworkCore;
using Task4ya.Application.Dtos;
using Task4ya.Application.Mappers;
using Task4ya.Infrastructure.Data;

namespace Task4ya.Application.TaskItem.Queries;

public class TaskItemsQueryHandler : IRequestHandler<GetAllTaskItemsQuery, PagedResponseDto<TaskItemDto>>, IRequestHandler<GetTaskItemByIdQuery, TaskItemDto>
{
	private readonly Task4YaDbContext _dbcontext;

	public TaskItemsQueryHandler(Task4YaDbContext dbcontext)
	{
		_dbcontext = dbcontext;
	}

	public async Task<PagedResponseDto<TaskItemDto>> Handle(GetAllTaskItemsQuery request, CancellationToken cancellationToken)
	{
		var query = _dbcontext.TaskItems.AsQueryable();

		if (!string.IsNullOrEmpty(request.SearchTerm))
		{
			query = query.Where(t => t.Title.Contains(request.SearchTerm));
		}

		if (!string.IsNullOrEmpty(request.SortBy))
		{
			query = request.SortDescending
				? query.OrderByDescending(t => EF.Property<object>(t, request.SortBy))
				: query.OrderBy(t => EF.Property<object>(t, request.SortBy));
		}

		var items = await query
			.Skip((request.Page - 1) * request.PageSize)
			.Take(request.PageSize)
			.Select(t => t.MapToDto())
			.ToListAsync(cancellationToken);
		
		return new PagedResponseDto<TaskItemDto>
		{
			Items = items,
			TotalCount = await query.CountAsync(cancellationToken),
			Page = request.Page,
			PageSize = request.PageSize
		};
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