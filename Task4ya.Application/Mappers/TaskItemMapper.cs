using Task4ya.Application.Dtos;

namespace Task4ya.Application.Mappers;

public static class TaskItemMapper
{
	public static Domain.Entities.TaskItem MapToEntity(this TaskItemDto itemDto)
	{
        ArgumentNullException.ThrowIfNull(itemDto);

        return new Domain.Entities.TaskItem(itemDto.Title, itemDto.Description, itemDto.DueDate);
	}
	
	public static TaskItemDto MapToDto(this Domain.Entities.TaskItem entity)
	{
		ArgumentNullException.ThrowIfNull(entity);

		return new TaskItemDto
		{
			Id = entity.Id,
			Title = entity.Title,
			Description = entity.Description,
			DueDate = entity.DueDate,
			Status = entity.Status,
			Priority = entity.Priority
		};
	}
	
}