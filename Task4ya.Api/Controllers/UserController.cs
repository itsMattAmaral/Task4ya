using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Task4ya.Api.Models.User;
using Task4ya.Application.Dtos;
using Task4ya.Application.User.Queries;

namespace Task4ya.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
	private readonly IMediator _mediator;
    private static readonly string[] SourceArray = new[] { "Id", "Name", "Email", "CreatedAt" };

    public UserController(IMediator mediator)
	{
		_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}
	
	[HttpPost]
	[Route("register")]
	[ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.Created)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult<UserDto>> AddUser([FromBody] AddUserModel? model)
	{
		if (
			model is null ||
			string.IsNullOrWhiteSpace(model.Name) ||
			string.IsNullOrWhiteSpace(model.Email) ||
			string.IsNullOrWhiteSpace(model.Password)
			)
		{
			return BadRequest("Invalid user data. Name, Email and Password cannot be empty.");
		}
		var command = model.GetCommand();

		try
		{
			var result = await _mediator.Send(command);
			return CreatedAtAction(nameof(AddUser), new {id = result.Id}, result);
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(ex.Message);
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
		}
	}
	
	[HttpGet]
	[ProducesResponseType(typeof(PagedResponseDto<UserDto>), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult<PagedResponseDto<UserDto>>> GetAllUsers(
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
			return BadRequest("Invalid sortBy parameter. Allowed values are: Name, Email, CreatedAt.");
		}
		if (sortDescending && sortBy is null)
		{
			return BadRequest("sortDescending can only be true if sortBy is provided.");
		}
		var query = new GetAllUsersQuery(page, pageSize, searchTerm, sortBy, sortDescending);
		var result = await _mediator.Send(query);
		
		return Ok(result);
	}
	
	[HttpGet("{id:int}")]
	[ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.NotFound)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult<UserDto>> GetUserById(int id)
	{
		if (id <= 0 || !int.TryParse(id.ToString(), out _))
		{
			return BadRequest("Invalid user ID.");
		}
		
		var query = new GetUserByIdQuery(id);
		var result = await _mediator.Send(query);
		if (result is null)
		{
			return NotFound($"User with ID {id} not found.");
		}

		return Ok(result);
	}
}