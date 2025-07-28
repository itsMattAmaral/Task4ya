using MediatR;
using Task4ya.Application.Dtos;
using Task4ya.Application.Mappers;
using Task4ya.Application.TaskItem.Commands.Actions;
using Task4ya.Domain.Repositories;
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
	private readonly IBoardRepository _boardRepository;
	public TaskItemCommandHandler(Task4YaDbContext dbcontext, IBoardRepository boardRepository, ITaskItemRepository taskItemRepository)
	{
		_boardRepository = boardRepository;
		_dbcontext = dbcontext;
	}
	
	public async Task<TaskItemDto> Handle(AddTaskItemCommand request, CancellationToken cancellationToken)
	{
		await ValidateBoardExists(request.BoardId);
		
		var newTask = new Domain.Entities.TaskItem(
			request.BoardId,
			request.Title,
			request.Description,
			request.DueDate,
			request.Priority,
			request.Status
		);
		_dbcontext.Add(newTask);
		await _dbcontext.SaveChangesAsync(cancellationToken);
		var board = await _boardRepository.GetByIdAsync(request.BoardId);
		board?.AddTaskItem(newTask);
		await _dbcontext.SaveChangesAsync(cancellationToken);
		return newTask.MapToDto();
	}
	
	public async Task<TaskItemDto> Handle(UpdateTaskItemCommand request, CancellationToken cancellationToken)
	{
        ArgumentNullException.ThrowIfNull(request);
        await ValidateBoardExists(request.BoardId);
        var task = await _dbcontext.TaskItems.FindAsync(request.Id, cancellationToken);
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

	public async Task<TaskItemDto?> Handle(UpdateTaskDueDateCommand request, CancellationToken cancellationToken)
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
	
	private async Task ValidateBoardExists(int boardId)
	{
		var board = await _boardRepository.GetByIdAsync(boardId);
		if (board == null)
		{
			throw new KeyNotFoundException($"Board with ID {boardId} not found.");
		}
	}
}