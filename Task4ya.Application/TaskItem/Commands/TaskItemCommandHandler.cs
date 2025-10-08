using MediatR;
using Task4ya.Application.Common.Commands;
using Task4ya.Application.Dtos;
using Task4ya.Application.Helpers;
using Task4ya.Application.Mappers;
using Task4ya.Application.TaskItem.Commands.Actions;
using Task4ya.Domain.Exceptions;
using Task4ya.Domain.Repositories;
using Task4ya.Infrastructure.Data;

namespace Task4ya.Application.TaskItem.Commands;

public class TaskItemCommandHandler(
	Task4YaDbContext dbcontext,
	IBoardRepository boardRepository,
	IUserRepository userRepository,
	ITaskItemRepository taskItemRepository,
	IMediator mediator)
	:
		IRequestHandler<AddTaskItemCommand, TaskItemDto>,
		IRequestHandler<UpdateTaskItemCommand, TaskItemDto>,
		IRequestHandler<UpdateTaskStatusCommand, TaskItemDto>,
		IRequestHandler<UpdateTaskPriorityCommand, TaskItemDto>,
		IRequestHandler<UpdateTaskDueDateCommand, TaskItemDto?>,
		IRequestHandler<UpdateTaskItemBoardIdCommand, TaskItemDto>,
		IRequestHandler<UpdateTaskItemAssigneeToIdCommand, TaskItemDto>,
		IRequestHandler<DeleteTaskItemCommand>
{
	public async Task<TaskItemDto> Handle(AddTaskItemCommand request, CancellationToken cancellationToken)
	{
		await ValidateBoardExists(request.BoardId);
		
		int? validatedAssigneeId = null;
        
		if (request.AssigneeToId is > 0)
		{
			var user = await userRepository.GetByIdAsync(request.AssigneeToId.Value);
			if (user is null) 
				throw new UserNotFoundException($"User with ID {request.AssigneeToId} does not exist.");
			validatedAssigneeId = request.AssigneeToId.Value;
		}
		
		var newTask = new Domain.Entities.TaskItem(
			request.BoardId,
			request.Title,
			request.Description,
			request.DueDate,
			request.Priority,
			request.Status,
			validatedAssigneeId
		);
		dbcontext.Add(newTask);
		await dbcontext.SaveChangesAsync(cancellationToken);
		var board = await boardRepository.GetByIdAsync(request.BoardId);
		board?.AddTaskItem(newTask);
		await dbcontext.SaveChangesAsync(cancellationToken);
		await InvalidateCachesAsync(
			Array.Empty<string>(), 
			new[] {CacheKeyGenerator.TaskitemsPrefix, CacheKeyGenerator.BoardsPrefix}, 
			cancellationToken);
		return newTask.MapToDto();
	}
	
	public async Task<TaskItemDto> Handle(UpdateTaskItemCommand request, CancellationToken cancellationToken)
	{
        ArgumentNullException.ThrowIfNull(request);
        await ValidateBoardExists(request.BoardId);
        int? validatedAssigneeId = null;
        
        if (request.AssigneeToId is > 0)
        {
	        var user = await userRepository.GetByIdAsync(request.AssigneeToId.Value);
	        if (user is null) 
		        throw new UserNotFoundException($"User with ID {request.AssigneeToId} does not exist.");
	        validatedAssigneeId = request.AssigneeToId.Value;
        }
        
        var task = await taskItemRepository.GetByIdAsync(request.Id);
		if (task == null)
		{
			throw new KeyNotFoundException($"Task with ID {request.Id} not found.");
		}

		task.UpdateTaskItem(
			newBoardId: request.BoardId,
			newTitle: request.Title,
			newDescription: request.Description,
			newDueDate: request.DueDate,
			newPriority: request.Priority,
			newStatus: request.Status,
			newAssigneeToId: validatedAssigneeId
		);

		await dbcontext.SaveChangesAsync(cancellationToken);
		await InvalidateCachesAsync(
			Array.Empty<string>(), 
			new[] {CacheKeyGenerator.TaskitemsPrefix, CacheKeyGenerator.BoardsPrefix}, 
			cancellationToken);
		return task.MapToDto();
	}
	
	public async Task<TaskItemDto> Handle(UpdateTaskItemBoardIdCommand request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		await ValidateBoardExists(request.NewBoardId);
		var task = await taskItemRepository.GetByIdAsync(request.Id);
		if (task == null)
		{
			throw new KeyNotFoundException($"Task with ID {request.Id} not found.");
		}
		if (task.BoardId == request.NewBoardId)
		{
			throw new InvalidOperationException($"Task with ID {request.Id} is already in board with ID {request.NewBoardId}.");
		}

		if (task.BoardId is not null)
		{
			var oldBoard = await boardRepository.GetByIdAsync(task.BoardId);
			oldBoard?.RemoveTaskItem(task);
			await dbcontext.SaveChangesAsync(cancellationToken);
		}
		var newBoard = await boardRepository.GetByIdAsync(request.NewBoardId);
		task.BoardId = request.NewBoardId;
		newBoard?.AddTaskItem(task);
		await dbcontext.SaveChangesAsync(cancellationToken);
		await InvalidateCachesAsync(
			Array.Empty<string>(), 
			new[] {CacheKeyGenerator.TaskitemsPrefix, CacheKeyGenerator.BoardsPrefix}, 
			cancellationToken);
		return task.MapToDto();
	}

	public async Task<TaskItemDto> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		var task = await taskItemRepository.GetByIdAsync(request.Id);
		if (task == null)
		{
			throw new KeyNotFoundException($"Task with ID {request.Id} not found.");
		}
		task.UpdateStatus(request.Status);
		await dbcontext.SaveChangesAsync(cancellationToken);
		await InvalidateCachesAsync(
			Array.Empty<string>(), 
			new[] {CacheKeyGenerator.TaskitemsPrefix, CacheKeyGenerator.BoardsPrefix}, 
			cancellationToken);
		return task.MapToDto();
	}

	public async Task<TaskItemDto> Handle(UpdateTaskPriorityCommand request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		var task = await taskItemRepository.GetByIdAsync(request.Id);
		if (task == null)
		{
			throw new KeyNotFoundException($"Task with ID {request.Id} not found.");
		}
		task.UpdatePriority(request.Priority);
		await dbcontext.SaveChangesAsync(cancellationToken);
		await InvalidateCachesAsync(
			Array.Empty<string>(), 
			new[] {CacheKeyGenerator.TaskitemsPrefix, CacheKeyGenerator.BoardsPrefix}, 
			cancellationToken);
		return task.MapToDto();
	}

	public async Task<TaskItemDto?> Handle(UpdateTaskDueDateCommand request, CancellationToken cancellationToken)
	{
		var task = await taskItemRepository.GetByIdAsync(request.Id);
		if (task == null)
		{
			throw new KeyNotFoundException($"Task with ID {request.Id} not found.");
		}
		task.UpdateDueDate(request.DueDate);
		await dbcontext.SaveChangesAsync(cancellationToken);
		await InvalidateCachesAsync(
			Array.Empty<string>(), 
			new[] {CacheKeyGenerator.TaskitemsPrefix, CacheKeyGenerator.BoardsPrefix}, 
			cancellationToken);
		return task.MapToDto();
	}

	public async Task Handle(DeleteTaskItemCommand request, CancellationToken cancellationToken)
	{
        ArgumentNullException.ThrowIfNull(request);
        
        var task = await taskItemRepository.GetByIdAsync(request.Id);

		if (task == null)
		{
			throw new KeyNotFoundException($"Task with ID {request.Id} not found.");
		}
		dbcontext.TaskItems.Remove(task);
		await dbcontext.SaveChangesAsync(cancellationToken);
		await InvalidateCachesAsync(
			Array.Empty<string>(), 
			new[] {CacheKeyGenerator.TaskitemsPrefix, CacheKeyGenerator.BoardsPrefix}, 
			cancellationToken);
    }
	
	private async Task ValidateBoardExists(int? boardId)
	{
		if (boardId != null)
		{
			var board = await boardRepository.GetByIdAsync(boardId);
			if (board == null) throw new KeyNotFoundException($"Board with ID {boardId} not found.");
		}
	}

	public async Task<TaskItemDto> Handle(UpdateTaskItemAssigneeToIdCommand request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		var task = await taskItemRepository.GetByIdAsync(request.Id);
		if (task == null)
		{
			throw new KeyNotFoundException($"Task with ID {request.Id} not found.");
		}
		if (request.NewAssigneeId <= 0) throw new ArgumentException("New assignee ID must be greater than 0.", nameof(request.NewAssigneeId));
		if (task.AssigneeToId == request.NewAssigneeId) throw new InvalidOperationException($"Task with ID {request.Id} is already assigned to user with ID {request.NewAssigneeId}.");
		
		var assigneeExists = await userRepository.GetByIdAsync(request.NewAssigneeId) != null;
		
		if (!assigneeExists) throw new UserNotFoundException("Assignee with the specified ID does not exist.");
		
		task.UpdateAssigneeToId(request.NewAssigneeId);
		await dbcontext.SaveChangesAsync(cancellationToken);
		await InvalidateCachesAsync(
			Array.Empty<string>(), 
			new[] {CacheKeyGenerator.TaskitemsPrefix, CacheKeyGenerator.BoardsPrefix}, 
			cancellationToken);
		return task.MapToDto();
	}
	
	private async Task InvalidateCachesAsync(string[] keys, string[] patterns, CancellationToken cancellationToken)
	{
		await mediator.Send(new InvalidateCacheCommand { Keys = keys, Patterns = patterns }, cancellationToken);
	}
}