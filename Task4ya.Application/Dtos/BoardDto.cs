namespace Task4ya.Application.Dtos;

public class BoardDto
{
	public int Id { get; init; }
	public int OwnerId { get; init; }
	public DateTime CreatedAt { get; init; }
	public DateTime UpdatedAt { get; init; }
	public required string Name { get; init; }
	public required ICollection<TaskItemDto> TaskGroup { get; init; }
}