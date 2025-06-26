using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Task4ya.Api.Models.TaskItem;
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
	[ProducesResponseType(typeof(TaskItemDto), (int)HttpStatusCode.Created)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult<TaskItemDto>> AddTaskItem([FromBody] AddTaskItemCommand command)
	{
		var result = await _mediator.Send(command);
		return CreatedAtAction(nameof(AddTaskItem), new { id = result.Id }, result);
	}

	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<TaskItemDto>), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetAllTaskItems()
	{
		var result = await _mediator.Send(new GetAllTaskItemsQuery());
		return Ok(result);
	}

	[HttpGet("{id}")]
	[ProducesResponseType(typeof(TaskItemDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
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
	[ProducesResponseType(typeof(TaskItemDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult> UpdateTaskItem([FromRoute]int id, [FromBody] UpdateTaskItemModel model)
	{
        ArgumentNullException.ThrowIfNull(model);
        var command = model.GetCommand(id);
        
		var result = await _mediator.Send(command);
		return Ok(result);
	}
	
	[HttpPatch("{id}/status")]
	[ProducesResponseType(typeof(TaskItemDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult> UpdateTaskStatus([FromRoute] int id, [FromBody] UpdateTaskStatusModel model, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(model);
		var command = model.GetCommand(id);
		var result = await _mediator.Send(command, cancellationToken);
		return Ok(result);
	}

	[HttpPatch("{id}/priority")]
	[ProducesResponseType(typeof(TaskItemDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult> UpdateTaskPriority([FromRoute] int id, [FromBody] UpdateTaskPriorityModel model,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(model);
		var command = model.GetCommand(id);
		var result = await _mediator.Send(command, cancellationToken);
		return Ok(result);
	}
	
	[HttpDelete("{id}")]
	[ProducesResponseType((int)HttpStatusCode.NoContent)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult> DeleteTaskItem([FromRoute] int id, CancellationToken cancellationToken = default)
	{
		var command = new DeleteTaskItemCommand(id);
		await _mediator.Send(command, cancellationToken);
		return NoContent();
	}
}