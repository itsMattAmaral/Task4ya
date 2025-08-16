using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.User.Commands.Actions;

public record UpdateUserCommand(string NewName, string NewEmail) : IRequest<UserDto>
{
	public int Id { get; set; }
}