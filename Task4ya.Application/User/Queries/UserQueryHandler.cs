using MediatR;
using Task4ya.Application.Dtos;
using Task4ya.Application.Mappers;
using Task4ya.Domain.Repositories;

namespace Task4ya.Application.User.Queries;

public class UserQueryHandler : IRequestHandler<GetAllUsersQuery, PagedResponseDto<UserDto>>
{
	private readonly IUserRepository _userRepository;
	
	public UserQueryHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
	}
	
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
}