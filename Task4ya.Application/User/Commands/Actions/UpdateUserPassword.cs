using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.User.Commands.Actions;

public record UpdateUserPassword(string OldPassword, string NewPassword) : IRequest<UserDto>
{
	public int Id { get; set; }
}