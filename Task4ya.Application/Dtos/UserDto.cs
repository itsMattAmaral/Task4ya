namespace Task4ya.Application.Dtos;

public class UserDto
{
	public int Id { get; init; }
	public required string Name { get; init; }
	public required string Email { get; init; }
	public DateTime CreatedAt { get; init; }
	public DateTime UpdatedAt { get; init; }
	
}