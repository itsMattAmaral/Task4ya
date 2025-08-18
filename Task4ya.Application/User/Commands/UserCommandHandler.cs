using MediatR;
using Task4ya.Application.Dtos;
using Task4ya.Application.Mappers;
using Task4ya.Application.User.Commands.Actions;
using Task4ya.Domain.Exceptions;
using Task4ya.Domain.Repositories;
using Task4ya.Domain.Utils;
using Task4ya.Infrastructure.Data;

namespace Task4ya.Application.User.Commands;

public class UserCommandHandler(Task4YaDbContext dbcontext, IUserRepository userRepository) :
	IRequestHandler<AddUserCommand, UserDto>,
	IRequestHandler<UpdateUserCommand, UserDto>,
	IRequestHandler<UpdateUserPassword, UserDto>
{
	public async Task<UserDto> Handle(AddUserCommand request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		var userPassword = PasswordHandler.HashPassword(request.Password);
		var newUser = new Domain.Entities.User(request.Name, request.Email, userPassword);
		if (await userRepository.ExistsAsync(newUser.Email, cancellationToken))
		{
			throw new InvalidOperationException($"User with email {newUser.Email} already exists.");
		}
		dbcontext.Add(newUser);
		await dbcontext.SaveChangesAsync(cancellationToken);
		return newUser.MapToDto();
	}

	public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		var user = await userRepository.GetByIdAsync(request.Id);
		if (user is null)
		{
			throw new UserNotFoundException($"User with ID {request.Id} does not exist.");
		}
		
		user.UpdateUserDetails(request.NewName, request.NewEmail);
		await dbcontext.SaveChangesAsync(cancellationToken);
		
		return user.MapToDto();
	}
	
	public async Task<UserDto> Handle(UpdateUserPassword request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		var user = await userRepository.GetByIdAsync(request.Id);
		if (user is null)
		{
			throw new UserNotFoundException($"User with ID {request.Id} does not exist.");
		}
		
		if (!PasswordHandler.VerifyPassword(request.OldPassword, user.Password))
		{
			throw new InvalidOperationException("Old password is incorrect.");
		}
		
		var newPassword = PasswordHandler.HashPassword(request.NewPassword);
		user.UpdatePassword(newPassword);
		await dbcontext.SaveChangesAsync(cancellationToken);
		
		return user.MapToDto();
	}
}