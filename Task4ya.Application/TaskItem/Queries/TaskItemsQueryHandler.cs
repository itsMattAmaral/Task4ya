using MediatR;
using Task4ya.Application.Dtos;
using Task4ya.Application.Mappers;
using Task4ya.Domain.Repositories;

namespace Task4ya.Application.TaskItem.Queries;

public class TaskItemsQueryHandler(ITaskItemRepository taskItemRepository) :
	IRequestHandler<GetAllTaskItemsQuery, PagedResponseDto<TaskItemDto>>,
	IRequestHandler<GetTaskItemByIdQuery, TaskItemDto?>
{
	private readonly ITaskItemRepository _taskItemRepository = taskItemRepository ?? throw new ArgumentNullException(nameof(taskItemRepository));

	public async Task<PagedResponseDto<TaskItemDto>> Handle(GetAllTaskItemsQuery request, CancellationToken cancellationToken)
	{
		var items = await _taskItemRepository.GetAllAsync(
			request.Page, 
			request.PageSize, 
			request.SearchTerm, 
			request.SortBy, 
			request.SortDescending);
		
		return new PagedResponseDto<TaskItemDto>
		{
			Items = items.Select(task => task.MapToDto()),
			TotalCount = await _taskItemRepository.GetCountAsync(request.SearchTerm),
			Page = request.Page,
			PageSize = request.PageSize
		};
	}
	
	public async Task<TaskItemDto?> Handle(GetTaskItemByIdQuery request, CancellationToken cancellationToken)
	{
		var taskItem = await _taskItemRepository.GetByIdAsync(request.Id);
		return taskItem?.MapToDto();
	}
}