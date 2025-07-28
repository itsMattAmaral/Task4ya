using MediatR;
using Task4ya.Application.Dtos;
using Task4ya.Application.Mappers;
using Task4ya.Domain.Repositories;

namespace Task4ya.Application.Board.Queries;

public class BoardQueryHandler : 
	IRequestHandler<GetAllBoardsQuery, PagedResponseDto<BoardDto>>,
	IRequestHandler<GetBoardByIdQuery, BoardDto?>
{
	private readonly IBoardRepository _boardRepository;
	
	public BoardQueryHandler(IBoardRepository boardRepository)
	{
		_boardRepository = boardRepository ?? throw new ArgumentNullException(nameof(boardRepository));
	}
	
	public async Task<PagedResponseDto<BoardDto>> Handle(GetAllBoardsQuery request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		
		var items = await _boardRepository.GetAllAsync(request.Page, request.PageSize, request.SearchTerm, request.SortBy, request.SortDescending);
		
		return new PagedResponseDto<BoardDto>
		{
			Items = items.Select(board => board.MapToDto()),
			TotalCount = await _boardRepository.GetCountAsync(request.SearchTerm),
			Page = request.Page,
			PageSize = request.PageSize
		};
	}
	
	public async Task<BoardDto?> Handle(GetBoardByIdQuery request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request, nameof(request));
		
		var board = await _boardRepository.GetByIdAsync(request.Id);
		return board?.MapToDto();
	}
}