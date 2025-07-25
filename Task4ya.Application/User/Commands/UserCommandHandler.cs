using MediatR;
using Task4ya.Application.Dtos;
using Task4ya.Application.Mappers;
using Task4ya.Application.User.Commands.Actions;
using Task4ya.Domain.Repositories;
using Task4ya.Infrastructure.Data;

namespace Task4ya.Application.User.Commands;

public class UserCommandHandler : 
	IRequestHandler<AddUserCommand, UserDto>
{
	private readonly Task4YaDbContext _dbcontext;
	private readonly IUserRepository _userRepository;
	
	public UserCommandHandler(Task4YaDbContext dbcontext, IUserRepository userRepository)
	{
		_dbcontext = dbcontext;
		_userRepository = userRepository;
	}

	public async Task<UserDto> Handle(AddUserCommand request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		var newUser = new Domain.Entities.User(request.Name, request.Email, request.Password);
		_dbcontext.Add(newUser);
		await _dbcontext.SaveChangesAsync(cancellationToken);
		return newUser.MapToDto();
	}
}