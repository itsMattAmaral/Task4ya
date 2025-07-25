using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.User.Commands.Actions;

public record AddUserCommand : IRequest<UserDto>
{
	public string Name { get; init; }
	public string Email { get; init; }
	public string Password { get; init; }
	
	public AddUserCommand(string name, string email, string password)
	{
		Name = name;
		Email = email;
		Password = password;
	}
}