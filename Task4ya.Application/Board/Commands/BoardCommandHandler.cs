using MediatR;
using Task4ya.Application.Board.Commands.Actions;
using Task4ya.Application.Dtos;
using Task4ya.Application.Mappers;
using Task4ya.Domain.Repositories;
using Task4ya.Infrastructure.Data;

namespace Task4ya.Application.Board.Commands;

public class BoardCommandHandler :
	IRequestHandler<AddBoardCommand, BoardDto>,
	IRequestHandler<DeleteBoardCommand>,
	IRequestHandler<AddTaskItemToBoardCommand>,
	IRequestHandler<RemoveTaskItemToBoardCommand>
{
	private readonly Task4YaDbContext _dbcontext;
	private readonly ITaskItemRepository _taskItemRepository;
	private readonly IBoardRepository _boardRepository;

	public BoardCommandHandler(Task4YaDbContext dbcontext, ITaskItemRepository taskItemRepository,
		IBoardRepository boardRepository)
	{
		_dbcontext = dbcontext;
		_taskItemRepository = taskItemRepository;
		_boardRepository = boardRepository;
	}

	public async Task<BoardDto> Handle(AddBoardCommand request, CancellationToken cancellationToken)
	{
		var newBoard = new Domain.Entities.Board(request.Name);
		_dbcontext.Add(newBoard);
		await _dbcontext.SaveChangesAsync(cancellationToken);
		foreach (var taskId in request.TaskItemIds)
		{
			var taskItem = await _taskItemRepository.GetByIdAsync(taskId);
			if (taskItem == null)
			{
				throw new KeyNotFoundException($"TaskItem with ID {taskId} not found.");
			}
			
			taskItem.BoardId = newBoard.Id;
			newBoard.AddTaskItem(taskItem);
		}
		await _dbcontext.SaveChangesAsync(cancellationToken);
		return newBoard.MapToDto();
	}

	public async Task Handle(AddTaskItemToBoardCommand request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);

		var board = await _boardRepository.GetByIdAsync(request.BoardId);
		if (board == null)
		{
			throw new KeyNotFoundException($"Board with ID {request.BoardId} not found.");
		}
		var taskItem = await _taskItemRepository.GetByIdAsync(request.TaskItemId);
		if (taskItem == null)
		{
			throw new KeyNotFoundException($"TaskItem with ID {request.TaskItemId} not found.");
		}
		if (board.TaskGroup.Any(t => t.Id == taskItem.Id))
		{
			throw new InvalidOperationException($"TaskItem with ID {taskItem.Id} already exists in the board.");
		}
		if (taskItem.BoardId != 0)
		{
			throw new InvalidOperationException($"TaskItem with ID {taskItem.Id} already belongs to another board.");
		}
		
		taskItem.BoardId = board.Id;
		board.AddTaskItem(taskItem);
		await _dbcontext.SaveChangesAsync(cancellationToken);
	}
	
	public async Task Handle(RemoveTaskItemToBoardCommand request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);

		var board = await _boardRepository.GetByIdAsync(request.BoardId);
		if (board == null)
		{
			throw new KeyNotFoundException($"Board with ID {request.BoardId} not found.");
		}
		if (board.TaskGroup.All(t => t.Id != request.TaskItemId))
		{
			throw new InvalidOperationException($"TaskItem with ID {request.TaskItemId} does not exist in the board.");
		}
		var taskItem = await _taskItemRepository.GetByIdAsync(request.TaskItemId);
		if (taskItem == null) 
		{
			throw new KeyNotFoundException($"TaskItem with ID {request.TaskItemId} not found.");
		}
		if (taskItem.BoardId != board.Id)
		{
			throw new InvalidOperationException($"TaskItem with ID {request.TaskItemId} does not belong to the board with ID {request.BoardId}.");
		}
		board.RemoveTaskItem(taskItem);
		taskItem.BoardId = 0;
		await _dbcontext.SaveChangesAsync(cancellationToken);
	}

	public async Task Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		var board = await _boardRepository.GetByIdAsync(request.Id);
		if (board == null)
		{
			throw new KeyNotFoundException($"Board with ID {request.Id} not found.");
		}
		if (board.TaskGroup.Count != 0)
		{
			var taskItems = board.TaskGroup.ToList();
			foreach (var taskItem in taskItems)
			{
				taskItem.BoardId = 0;
				await _taskItemRepository.UpdateAsync(taskItem);
			}
			board.ClearTaskItems();
		}
		await _boardRepository.DeleteAsync(request.Id);
		await _dbcontext.SaveChangesAsync(cancellationToken);
	}
}