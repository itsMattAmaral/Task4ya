using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.User.Queries;

public record GetUserByIdQuery(int Id) : IRequest<UserDto?>;