using MediatR;
using Task4ya.Application.Dtos;
using Task4ya.Application.Mappers;
using Task4ya.Application.TaskItem.Commands.Actions;
using Task4ya.Infrastructure.Data;

namespace Task4ya.Application.TaskItem.Commands;

public class TaskItemCommandHandler : 
	IRequestHandler<AddTaskItemCommand, TaskItemDto>,
	IRequestHandler<UpdateTaskItemCommand, TaskItemDto>,
	IRequestHandler<UpdateTaskStatusCommand, TaskItemDto>,
	IRequestHandler<UpdateTaskPriorityCommand, TaskItemDto>,
	IRequestHandler<DeleteTaskItemCommand>
{
	private readonly Task4YaDbContext _dbcontext;
	public TaskItemCommandHandler(Task4YaDbContext dbcontext)
		{
			_dbcontext = dbcontext;
		}
	
	public async Task<TaskItemDto> Handle(AddTaskItemCommand request, CancellationToken cancellationToken)
	{
		var newTask = new Domain.Entities.TaskItem(
			request.Title,
			request.Description,
			request.DueDate,
			request.Priority,
			request.Status
		);
		_dbcontext.Add(newTask);
		await _dbcontext.SaveChangesAsync(cancellationToken);
		return newTask.MapToDto();
	}
	
	public async Task<TaskItemDto> Handle(UpdateTaskItemCommand request, CancellationToken cancellationToken)
	{
        ArgumentNullException.ThrowIfNull(request);
        var task = await _dbcontext.TaskItems.FindAsync(request.Id, cancellationToken);
		if (task == null)
		{
			throw new KeyNotFoundException($"Task with ID {request.Id} not found.");
		}

		task.UpdateTaskItem(
			newTitle: request.Title,
			newDescription: request.Description,
			newDueDate: request.DueDate,
			newPriority: request.Priority,
			newStatus: request.Status
		);

		await _dbcontext.SaveChangesAsync(cancellationToken);
		return task.MapToDto();
	}

	public async Task<TaskItemDto> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		var task = await _dbcontext.TaskItems.FindAsync(request.Id, cancellationToken);
		if (task == null)
		{
			throw new KeyNotFoundException($"Task with ID {request.Id} not found.");
		}
		task.UpdateStatus(request.Status);
		await _dbcontext.SaveChangesAsync(cancellationToken);
		return task.MapToDto();
	}

	public async Task<TaskItemDto> Handle(UpdateTaskPriorityCommand request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		var task = await _dbcontext.TaskItems.FindAsync(request.Id, cancellationToken);
		if (task == null)
		{
			throw new KeyNotFoundException($"Task with ID {request.Id} not found.");
		}
		task.UpdatePriority(request.Priority);
		await _dbcontext.SaveChangesAsync(cancellationToken);
		return task.MapToDto();
	}

	public async Task<TaskItemDto> Handle(UpdateTaskDueDateCommand request, CancellationToken cancellationToken)
	{
		var task = await _dbcontext.TaskItems.FindAsync(request.Id, cancellationToken);
		if (task == null)
		{
			throw new KeyNotFoundException($"Task with ID {request.Id} not found.");
		}
		task.UpdateDueDate(request.DueDate);
		await _dbcontext.SaveChangesAsync(cancellationToken);
		return task.MapToDto();
	}

	public async Task Handle(DeleteTaskItemCommand request, CancellationToken cancellationToken)
	{
        ArgumentNullException.ThrowIfNull(request);
        
		var task = await _dbcontext.TaskItems.FindAsync(request.Id, cancellationToken);

		if (task == null)
		{
			throw new KeyNotFoundException($"Task with ID {request.Id} not found.");
		}
		_dbcontext.TaskItems.Remove(task);
		await _dbcontext.SaveChangesAsync(cancellationToken);
    }
}