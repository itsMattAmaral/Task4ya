using Task4ya.Application.Dtos;

namespace Task4ya.Application.Mappers;

public static class BoardMapper
{
	public static BoardDto MapToDto(this Domain.Entities.Board board)
	{
		ArgumentNullException.ThrowIfNull(board);
		
		return new BoardDto
		{
			Id = board.Id,
			Name = board.Name,
			TaskGroup = board.TaskGroup.Select(task=> task.MapToDto()).ToList()
		};
	}
}