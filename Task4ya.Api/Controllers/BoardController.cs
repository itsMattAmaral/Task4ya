using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task4ya.Api.Models.Board;
using Task4ya.Application.Board.Commands.Actions;
using Task4ya.Application.Board.Queries;
using Task4ya.Application.Dtos;

namespace Task4ya.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardController : ControllerBase
{
	private readonly IMediator _mediator;
    private static readonly string[] SourceArray = new[] { "Name", "CreatedAt" };

    public BoardController(IMediator mediator)
	{
		_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}
	
	[Authorize(Policy = "AdminOrManager")]
	[HttpPost]
	[ProducesResponseType(typeof(BoardDto), (int)HttpStatusCode.Created)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult<BoardDto>> AddBoard([FromBody] AddBoardModel? model)
	{
		if (model is null || string.IsNullOrWhiteSpace(model.Name))
		{
			return BadRequest("Invalid board data. Name cannot be empty.");
		}
		var command = model.GetCommand();

		try
		{
			var result = await _mediator.Send(command);
			return CreatedAtAction(nameof(AddBoard), new { id = result.Id }, result);	
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(ex.Message);
		}
		catch (ArgumentException ex)
		{
			return BadRequest(ex.Message);
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred while adding the board: {ex.Message}");
		}
	}
	
	[Authorize(Policy = "AdminOrManager")]
	[HttpPost("{boardId:int}/AddTaskItemToBoard")]
	[ProducesResponseType((int)HttpStatusCode.NoContent)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<IActionResult> AddTaskItemToBoard([FromRoute] int boardId, [FromBody] AddTaskItemToBoardModel? model)
	{
		if (model is null || boardId <= 0 || model.TaskItemId <= 0)
		{
			return BadRequest("Invalid board or task item data. BoardId and TaskItemId must be greater than 0.");
		}
		var command = model.GetCommand(boardId);

		try
		{
			await _mediator.Send(command);
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(ex.Message);
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(ex.Message);
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred while adding the task item to the board: {ex.Message}");
		}
		
		return NoContent();
	}
	
	[Authorize(Policy = "AnyUser")]
	[HttpGet]
	[ProducesResponseType(typeof(PagedResponseDto<BoardDto>), (int)HttpStatusCode.OK)]
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
		if (page < 1 || pageSize < 1)
		{
			return BadRequest("Page and PageSize must be greater than 0.");
		}
		if (sortBy != null && !SourceArray.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
		{
			return BadRequest("Invalid sortBy parameter. Allowed values are: Name, CreatedAt.");
		}
		if (sortDescending && sortBy == null)
		{
			return BadRequest("sortDescending can only be true if sortBy is specified.");
		}
		if (searchTerm is {Length: > 100})
		{
			return BadRequest("Search term cannot exceed 100 characters.");
		}
		var query = new GetAllBoardsQuery(page, pageSize, searchTerm, sortBy, sortDescending);
		var result = await _mediator.Send(query);
		return Ok(result);
	}
	
	[Authorize(Policy = "AnyUser")]
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
		if (result is null)
		{
			return NotFound($"Board with ID {id} not found.");
		}
		
		return Ok(result);
	}
	
	[Authorize(Policy = "AdminOrManager")]
	[HttpPut("{id}")]
	[ProducesResponseType(typeof(BoardDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult<BoardDto>> UpdateBoardName([FromRoute] int id, [FromBody] UpdateBoardNameModel? model)
	{
		if (id <= 0 || model is null || string.IsNullOrWhiteSpace(model.NewName))
		{
			return BadRequest("Invalid board ID or name.");
		}
		
		var command = new UpdateBoardNameCommand(id, model.NewName);
		
		try
		{
			var result = await _mediator.Send(command);
			return Ok(result);
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(ex.Message);
		}
		catch (ArgumentException ex)
		{
			return BadRequest(ex.Message);
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred while updating the board name: {ex.Message}");
		}
	}
	[Authorize(Policy = "AdminOrManager")]
	[HttpPut("{boardId:int}/ChangeOwner/{newOwnerId:int}")]
	[ProducesResponseType((int)HttpStatusCode.NoContent)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<IActionResult> ChangeBoardOwner([FromRoute] int boardId, int newOwnerId)
	{
		if (boardId <= 0)
		{
			return BadRequest("Invalid board ID.");
		}
		if (newOwnerId <= 0)
		{
			return BadRequest("Invalid new owner ID.");
		}
		
		var command = new UpdateBoardOwnerCommand(boardId, newOwnerId);
		
		try
		{
			await _mediator.Send(command);
			return NoContent();
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(ex.Message);
		}
		catch (ArgumentException ex)
		{
			return BadRequest(ex.Message);
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(ex.Message);
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred while changing the board owner: {ex.Message}");
		}
	}
	
	[Authorize(Policy = "AdminOrManager")]
	[HttpDelete("{id:int}")]
	[ProducesResponseType((int)HttpStatusCode.NoContent)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<IActionResult> DeleteBoard([FromRoute] int id)
	{
		if (id <= 0 || !int.TryParse(id.ToString(), out _))
		{
			return BadRequest("Invalid board ID.");
		}
		var board = await _mediator.Send(new GetBoardByIdQuery(id));
		if (board is null)
		{
			return NotFound($"Board with ID {id} not found.");
		}
		var command = new DeleteBoardCommand(id);
		await _mediator.Send(command);
		return NoContent();
	}
	
}