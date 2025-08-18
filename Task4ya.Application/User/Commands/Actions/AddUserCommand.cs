using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.User.Commands.Actions;

public record AddUserCommand(string Name, string Email, string Password, List<string> Roles) : IRequest<UserDto>;