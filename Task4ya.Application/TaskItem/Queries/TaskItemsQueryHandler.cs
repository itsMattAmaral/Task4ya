using MediatR;
using Microsoft.Extensions.Configuration;
using Task4ya.Application.Dtos;
using Task4ya.Application.Helpers;
using Task4ya.Application.Mappers;
using Task4ya.Application.Services;
using Task4ya.Domain.Repositories;

namespace Task4ya.Application.TaskItem.Queries;

public class TaskItemsQueryHandler(ITaskItemRepository taskItemRepository, ICacheService cacheService, IConfiguration configuration) :
	IRequestHandler<GetAllTaskItemsQuery, PagedResponseDto<TaskItemDto>>,
	IRequestHandler<GetTaskItemByIdQuery, TaskItemDto?>
{
	private readonly ITaskItemRepository _taskItemRepository = taskItemRepository ?? throw new ArgumentNullException(nameof(taskItemRepository));

	public async Task<PagedResponseDto<TaskItemDto>> Handle(GetAllTaskItemsQuery request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		
		var cacheKey =
			CacheKeyGenerator.GetTaskItemsKey(request.Page, request.PageSize, request.SortBy, request.SortDescending, request.SearchTerm);
		var cachedTasks = await cacheService.GetAsync<PagedResponseDto<TaskItemDto>>(cacheKey);
		if (cachedTasks != null) return cachedTasks;
		
		var items = await _taskItemRepository.GetAllAsync(
			request.Page, 
			request.PageSize, 
			request.SearchTerm, 
			request.SortBy, 
			request.SortDescending);
		var tasks = new PagedResponseDto<TaskItemDto>
		{
			Items = items.Select(task => task.MapToDto()),
			TotalCount = await _taskItemRepository.GetCountAsync(request.SearchTerm),
			Page = request.Page,
			PageSize = request.PageSize
		};
		var expirationMinutes = int.TryParse(configuration["CacheSettings:TaskItemsCacheExpirationMinutes"], out var minutes) ? minutes : 60;
		await cacheService.SetAsync(cacheKey, tasks, TimeSpan.FromMinutes(expirationMinutes));

		return tasks;
	}
	
	public async Task<TaskItemDto?> Handle(GetTaskItemByIdQuery request, CancellationToken cancellationToken)
	{
		var taskItem = await _taskItemRepository.GetByIdAsync(request.Id);
		return taskItem?.MapToDto();
	}
}