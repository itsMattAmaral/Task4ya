using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.User.Queries;

public class GetUserByIdQuery(int id) : IRequest<UserDto?>
{
	public int Id { get; } = id;
}