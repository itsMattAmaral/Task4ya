using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task4ya.Api.Models.TaskItem;
using Task4ya.Application.Dtos;
using Task4ya.Application.TaskItem.Commands.Actions;
using Task4ya.Application.TaskItem.Queries;
using Task4ya.Domain.Enums;
using Task4ya.Domain.Exceptions;

namespace Task4ya.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskItemController : ControllerBase
{
	private readonly IMediator _mediator;
    private static readonly string[] SourceArray = new[] { "Title", "Priority", "DueDate", "Status", "CreatedAt" };

    public TaskItemController(IMediator mediator)
	{
		_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}
	
	[Authorize(Policy = "AnyUser")]
	[HttpPost]
	[ProducesResponseType(typeof(TaskItemDto), (int)HttpStatusCode.Created)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult<TaskItemDto>> AddTaskItem([FromBody] AddTaskItemModel model)
	{
		if (model.BoardId <= 0 ) BadRequest("Invalid task item. BoardId must be greater than 0");
		if (string.IsNullOrWhiteSpace(model.Title)) BadRequest("Invalid task item. Title cannot be empty.");
		var command = model.GetCommand();

		try
		{
			var result = await _mediator.Send(command);
			return CreatedAtAction(nameof(AddTaskItem), new {id = result.Id}, result);
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(ex.Message);
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError,
				$"An error occurred while adding the task item: {ex.Message}");
		}
	}
	
	[Authorize(Policy = "AnyUser")]
	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<TaskItemDto>), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult<PagedResponseDto<TaskItemDto>>> GetAllTaskItems(
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
			return BadRequest("Invalid sortBy parameter. Allowed values are: CreatedAt, Title, Priority, DueDate, Status.");
		}
		if (sortDescending && sortBy is null)
		{
			return BadRequest("sortDescending can only be true if sortBy is specified.");
		}
		if (searchTerm is {Length: > 100})
		{
			return BadRequest("Search term cannot exceed 100 characters.");
		}
		var query = new GetAllTaskItemsQuery(page, pageSize, searchTerm, sortBy, sortDescending);
		var result = await _mediator.Send(query);
		return Ok(result);
	}
	
	[Authorize(Policy = "AnyUser")]
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
		if (result is null)
		{
			return NotFound($"Task item with ID {id} not found.");
		}
		return Ok(result);
	}
	
	[Authorize(Policy = "AnyUser")]
	[HttpPut("{id}")]
	[ProducesResponseType(typeof(TaskItemDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult> UpdateTaskItem([FromRoute]int id, [FromBody] UpdateTaskItemModel? model)
	{
		if (model is null || id <= 0)
		{
			return BadRequest("Invalid task item data. ID must be greater than 0. Request body cannot be empty.");
		}
        var command = model.GetCommand(id);

        try
        {
	        var result = await _mediator.Send(command);
	        return Ok(result);
        }
        catch (KeyNotFoundException ex)
		{
	        return NotFound(ex.Message);
		}
		catch (Exception ex)
		{
	        return StatusCode((int)HttpStatusCode.InternalServerError,
		        $"An error occurred while updating the task item: {ex.Message}");
		}
	}
	
	[Authorize(Policy = "AnyUser")]
	[HttpPatch("{id}/status")]
	[ProducesResponseType(typeof(TaskItemDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult> UpdateTaskStatus([FromRoute] int id, [FromBody] UpdateTaskStatusModel? model, CancellationToken cancellationToken = default)
	{
		if (model is null || id <= 0)
		{
			return BadRequest("Invalid task item data. ID must be greater than 0. Request body cannot be empty.");
		}
		if (!Enum.IsDefined(typeof(TaskItemStatus), model.Status))
		{
			return BadRequest("Invalid task status. Allowed values are: Pending = 0, InProgress = 1, Blocked = 2, Done = 3.");
		}
		var command = model.GetCommand(id);

		try
		{
			var result = await _mediator.Send(command, cancellationToken);
			return Ok(result);
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(ex.Message);
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError,
				$"An error occurred while updating the task status: {ex.Message}");
		}
	}
	
	[Authorize(Policy = "AnyUser")]
	[HttpPatch("{id}/priority")]
	[ProducesResponseType(typeof(TaskItemDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult> UpdateTaskPriority([FromRoute] int id, [FromBody] UpdateTaskPriorityModel? model,
		CancellationToken cancellationToken = default)
	{
		if (model is null || id <= 0)
		{
			return BadRequest("Invalid task item data. ID must be greater than 0. Request body cannot be empty.");
		}
		if (!Enum.IsDefined(typeof(TaskItemPriority), model.Priority))
		{
			return BadRequest("Invalid task priority. Allowed values are: Low = 0, Medium = 1, High = 2, Urgent = 3.");
		}
		var command = model.GetCommand(id);

		try
		{
			var result = await _mediator.Send(command, cancellationToken);
			return Ok(result);
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(ex.Message);
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError,
				$"An error occurred while updating the task priority: {ex.Message}");
		}
	}
	
	[Authorize(Policy = "AnyUser")]
	[HttpPatch("{id}/DueDate")]
	[ProducesResponseType(typeof(TaskItemDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult> UpdateTaskDueDate([FromRoute] int id, [FromBody] UpdateTaskDueDateModel? model,
		CancellationToken cancellationToken = default)
	{
		if (model is null || id <= 0)
		{
			return BadRequest("Invalid task item data. ID must be greater than 0. Request body cannot be empty.");
		}
		if (model.DueDate < DateTime.UtcNow)
		{
			return BadRequest("Due date cannot be in the past.");
		}
		
		var command = model.GetCommand(id);

		try
		{
			var result = await _mediator.Send(command, cancellationToken);
			return Ok(result);
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(ex.Message);
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError,
				$"An error occurred while updating the task due date: {ex.Message}");
		}

	}
	[Authorize(Policy = "AnyUser")]
	[HttpPatch("{id}/assignee")]
	[ProducesResponseType(typeof(TaskItemDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult> UpdateTaskAssignee([FromRoute] int id, [FromBody] UpdateTaskItemAssigneeModel model)
	{
		if (id <= 0 || model.NewAssigneeId <= 0)
		{
			return BadRequest("Invalid task item ID or assignee ID. Both must be greater than 0.");
		}
		
		var command = model.GetCommand();
		command.Id = id;

		try
		{
			var result = await _mediator.Send(command);
			return Ok(result);
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(ex.Message);
		}
		catch (UserNotFoundException ex)
		{
			return NotFound(ex.Message);
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError,
				$"An error occurred while updating the task assignee: {ex.Message}");
		}
	}
	
	[Authorize(Policy = "AdminOrManager")]
	[HttpDelete("{id}")]
	[ProducesResponseType((int)HttpStatusCode.NoContent)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<ActionResult> DeleteTaskItem([FromRoute] int id, CancellationToken cancellationToken = default)
	{
		if (id <= 0 || !int.TryParse(id.ToString(), out _))
		{
			return BadRequest("Invalid task item ID.");
		}
		var taskItem = await _mediator.Send(new GetTaskItemByIdQuery(id), cancellationToken);
		if (taskItem is null)
		{
			return NotFound($"Task item with ID {id} not found.");
		}
		var command = new DeleteTaskItemCommand(id);
		await _mediator.Send(command, cancellationToken);
		return NoContent();
	}
}