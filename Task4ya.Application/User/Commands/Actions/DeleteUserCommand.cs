using MediatR;

namespace Task4ya.Application.User.Commands.Actions;

public record DeleteUserCommand(int Id) : IRequest;