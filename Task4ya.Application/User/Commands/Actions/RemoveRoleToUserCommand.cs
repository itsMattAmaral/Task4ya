using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.User.Commands.Actions;

public record RemoveRoleToUserCommand(string Role) : IRequest<UserDto>
{
	public int UserId { get; set; }
}