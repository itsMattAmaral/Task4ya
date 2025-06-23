using MediatR;
using Microsoft.AspNetCore.Mvc;
using Task4ya.Application.Dtos;
using Task4ya.Application.TaskItem.Commands.Actions;
using Task4ya.Application.TaskItem.Queries;

namespace Task4ya.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskItemController : ControllerBase
{
	private readonly IMediator _mediator;

	public TaskItemController(IMediator mediator)
	{
		_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}

	[HttpPost]
	public async Task<ActionResult<TaskItemDto>> AddTaskItem([FromBody] AddTaskItemCommand command)
	{
		var result = await _mediator.Send(command);
		return CreatedAtAction(nameof(AddTaskItem), new { id = result.Id }, result);
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetAllTaskItems()
	{
		var result = await _mediator.Send(new GetAllTaskItemsQuery());
		return Ok(result);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<TaskItemDto>> GetTaskItemById(int id)
	{
		if (id <= 0)
		{
			return BadRequest("Invalid task item ID.");
		}
		
		var result = await _mediator.Send(new GetTaskItemByIdQuery(id));
		return Ok(result);
	}
	
	[HttpPut("{id}")]
	public async Task<ActionResult<TaskItemDto>> UpdateTaskItem(int id, [FromBody] UpdateTaskItemCommand command)
	{
		if (id != command.Id && command.Id != 0)
		{
			return BadRequest("Invalid task item ID.");
		}
		
		var updatedCommand = command with { Id = id };
		var result = await _mediator.Send(updatedCommand);
		return Ok(result);
	}
}