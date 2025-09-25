using MediatR;
using Microsoft.Extensions.Configuration;
using Task4ya.Application.Dtos;
using Task4ya.Application.Helpers;
using Task4ya.Application.Mappers;
using Task4ya.Application.Services;
using Task4ya.Domain.Repositories;

namespace Task4ya.Application.Board.Queries;

public class BoardQueryHandler(IBoardRepository boardRepository, ICacheService cacheService, IConfiguration configuration)
	:
		IRequestHandler<GetAllBoardsQuery, PagedResponseDto<BoardDto>>,
		IRequestHandler<GetBoardByIdQuery, BoardDto?>
{
	private readonly IBoardRepository _boardRepository = boardRepository ?? throw new ArgumentNullException(nameof(boardRepository));

	public async Task<PagedResponseDto<BoardDto>> Handle(GetAllBoardsQuery request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		
		var cacheKey =
			CacheKeyGenerator.GetBoardsKey(request.Page, request.PageSize, request.SortBy, request.SortDescending, request.SearchTerm);
		var cachedBoards = await cacheService.GetAsync<PagedResponseDto<BoardDto>>(cacheKey);
		if (cachedBoards != null) return cachedBoards;
		
		var items = await _boardRepository.GetAllAsync(request.Page, request.PageSize, request.SearchTerm, request.SortBy, request.SortDescending);
		var boards = new PagedResponseDto<BoardDto>	{
			Items = items.Select(board => board.MapToDto()),
			TotalCount = await _boardRepository.GetCountAsync(request.SearchTerm),
			Page = request.Page,
			PageSize = request.PageSize
		};
		var expirationMinutes = int.TryParse(configuration["CacheSettings:BoardsCacheExpirationMinutes"], out var minutes) ? minutes : 60;
		await cacheService.SetAsync(cacheKey, boards, TimeSpan.FromMinutes(expirationMinutes));

		return boards;
	}
	
	public async Task<BoardDto?> Handle(GetBoardByIdQuery request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request, nameof(request));
		
		var board = await _boardRepository.GetByIdAsync(request.Id);
		return board?.MapToDto();
	}
}