using Task4ya.Application.Dtos;

namespace Task4ya.Application.Mappers;

public static class TaskItemMapper
{
	
	public static TaskItemDto MapToDto(this Domain.Entities.TaskItem entity)
	{
		ArgumentNullException.ThrowIfNull(entity);

		return new TaskItemDto
		{
			Id = entity.Id,
			BoardId = entity.BoardId,
			Title = entity.Title,
			Description = entity.Description,
			DueDate = entity.DueDate,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt,
			Status = entity.Status,
			Priority = entity.Priority
		};
	}
	
}