namespace Task4ya.Application.Dtos;

public class BoardDto
{
	public int Id { get; init; }
	public required string Name { get; init; }
	public required ICollection<TaskItemDto> TaskGroup { get; init; }
}