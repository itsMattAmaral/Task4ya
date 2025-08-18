using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.User.Queries;

public record GetAllUsersQuery(int Page = 1, int PageSize = 10, string? SearchTerm = null, string? SortBy = null, bool SortDescending = false) : IRequest<PagedResponseDto<UserDto>>;