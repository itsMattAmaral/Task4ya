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
	
	public UserController(IMediator mediator)
	{
		_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}
	
	[HttpPost]
	[ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.Created)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult<UserDto>> AddUser([FromBody] AddUserModel model)
	{
		ArgumentNullException.ThrowIfNull(model);
		var command = model.GetCommand();
		var result = await _mediator.Send(command);
		
		return CreatedAtAction(nameof(AddUser), new { id = result.Id }, result);
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
		var query = new GetAllUsersQuery(page, pageSize, searchTerm, sortBy, sortDescending);
		var result = await _mediator.Send(query);
		
		return Ok(result);
	}
}