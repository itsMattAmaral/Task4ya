using MediatR;
using Task4ya.Application.Dtos;
using Task4ya.Application.Mappers;
using Task4ya.Domain.Exceptions;
using Task4ya.Domain.Repositories;

namespace Task4ya.Application.User.Queries;

public class UserQueryHandler(IUserRepository userRepository) :
	IRequestHandler<GetAllUsersQuery, PagedResponseDto<UserDto>>,
	IRequestHandler<GetUserByIdQuery, UserDto?>
{
	private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

	public async Task<PagedResponseDto<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		
		var items = await _userRepository.GetAllAsync(request.Page, request.PageSize, request.SearchTerm, request.SortBy, request.SortDescending);
		
		return new PagedResponseDto<UserDto>
		{
			Items = items.Select(user => user.MapToDto()),
			TotalCount = await _userRepository.GetCountAsync(request.SearchTerm),
			Page = request.Page,
			PageSize = request.PageSize
		};
	}

	public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request, nameof(request));
		
		var user = await _userRepository.GetByIdAsync(request.Id);
		if (user is null) throw new UserNotFoundException();
		
		return user.MapToDto();
	}
}