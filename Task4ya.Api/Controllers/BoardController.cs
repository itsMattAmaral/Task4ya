using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Task4ya.Api.Models.Board;
using Task4ya.Application.Board.Commands.Actions;
using Task4ya.Application.Board.Queries;
using Task4ya.Application.Dtos;
using Task4ya.Domain.Repositories;
using Task4ya.Infrastructure.Data;

namespace Task4ya.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly IBoardRepository _boardRepository;
	private readonly Task4YaDbContext _dbContext;
	
	public BoardController(IMediator mediator, IBoardRepository boardRepository, Task4YaDbContext dbContext)
	{
		_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		_boardRepository = boardRepository ?? throw new ArgumentNullException(nameof(boardRepository));
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}
	
	[HttpPost]
	[ProducesResponseType(typeof(BoardDto), (int)HttpStatusCode.Created)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult<BoardDto>> AddBoard([FromBody] AddBoardModel model)
	{
		ArgumentNullException.ThrowIfNull(model);
		var command = model.GetCommand();
		var result = await _mediator.Send(command);
		
		return CreatedAtAction(nameof(AddBoard), new { id = result.Id }, result);
	}
	
	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<BoardDto>), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult<PagedResponseDto<BoardDto>>> GetAllBoards(
		[FromQuery] int page = 1, 
		[FromQuery] int pageSize = 10, 
		[FromQuery] string? searchTerm = null, 
		[FromQuery] string? sortBy = null, 
		[FromQuery] bool sortDescending = false)
	{
		var query = new GetAllBoardsQuery(page, pageSize, searchTerm, sortBy, sortDescending);
		var result = await _mediator.Send(query);
		return Ok(result);
	}
	
	[HttpGet("{id}")]
	[ProducesResponseType(typeof(BoardDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult<BoardDto>> GetBoardById(int id)
	{
		if (id <= 0)
		{
			return BadRequest("Invalid board ID.");
		}

		var query = new GetBoardByIdQuery(id);
		var result = await _mediator.Send(query);
		
		return Ok(result);
	}
	
	[HttpDelete("{id}")]
	[ProducesResponseType((int)HttpStatusCode.NoContent)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<IActionResult> DeleteBoard([FromRoute] int id)
	{
		if (id <= 0)
		{
			return BadRequest("Invalid board ID.");
		}

		var command = new DeleteBoardCommand(id);
		await _mediator.Send(command);
		return NoContent();
	}
	
}