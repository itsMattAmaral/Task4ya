using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.Board.Queries;

public class GetAllBoardsQuery : IRequest<PagedResponseDto<BoardDto>>
{
	public int Page { get; set; }
	public int PageSize { get; set; }
	public string? SearchTerm { get; set; }
	public string? SortBy { get; set; }
	public bool SortDescending { get; set; } = false;
	
	public GetAllBoardsQuery(int page = 1, int pageSize = 10, string? searchTerm = null, string? sortBy = null, bool sortDescending = false)
	{
		Page = page;
		PageSize = pageSize;
		SearchTerm = searchTerm;
		SortBy = sortBy;
		SortDescending = sortDescending;
	}
}