using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.Board.Queries;

public record GetAllBoardsQuery(int Page = 1, int PageSize = 10, string? SearchTerm = null, string? SortBy = null, bool SortDescending = false) : IRequest<PagedResponseDto<BoardDto>>;