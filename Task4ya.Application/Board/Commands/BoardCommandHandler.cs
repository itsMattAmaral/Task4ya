using MediatR;
using Task4ya.Application.Board.Commands.Actions;
using Task4ya.Application.Dtos;
using Task4ya.Application.Mappers;
using Task4ya.Domain.Repositories;
using Task4ya.Infrastructure.Data;

namespace Task4ya.Application.Board.Commands;

public class BoardCommandHandler(
	Task4YaDbContext dbcontext,
	ITaskItemRepository taskItemRepository,
	IBoardRepository boardRepository)
	:
		IRequestHandler<AddBoardCommand, BoardDto>,
		IRequestHandler<DeleteBoardCommand>,
		IRequestHandler<AddTaskItemToBoardCommand>,
		IRequestHandler<UpdateBoardNameCommand, BoardDto>,
		IRequestHandler<RemoveTaskItemToBoardCommand>
{
	public async Task<BoardDto> Handle(AddBoardCommand request, CancellationToken cancellationToken)
	{
		var newBoard = new Domain.Entities.Board(request.Name);
		dbcontext.Add(newBoard);
		await dbcontext.SaveChangesAsync(cancellationToken);

		foreach (var taskId in request.TaskItemIds)
		{
			var taskItem = await taskItemRepository.GetByIdAsync(taskId);

			if (taskItem == null)
			{
				throw new KeyNotFoundException($"TaskItem with ID {taskId} not found.");
			}
			if (taskItem.BoardId != 0)
			{
				throw new InvalidOperationException($"TaskItem with ID {taskId} already belongs to another board.");
			}

			taskItem.BoardId = newBoard.Id;
			newBoard.AddTaskItem(taskItem);
		}
		await dbcontext.SaveChangesAsync(cancellationToken);
		return newBoard.MapToDto();
	}

	public async Task Handle(AddTaskItemToBoardCommand request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);

		var board = await boardRepository.GetByIdAsync(request.BoardId);

		if (board == null)
		{
			throw new KeyNotFoundException($"Board with ID {request.BoardId} not found.");
		}
		var taskItem = await taskItemRepository.GetByIdAsync(request.TaskItemId);

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
		await dbcontext.SaveChangesAsync(cancellationToken);
	}

	public async Task<BoardDto> Handle(UpdateBoardNameCommand request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);

		var board = await boardRepository.GetByIdAsync(request.BoardId);

		if (board == null)
		{
			throw new KeyNotFoundException($"Board with ID {request.BoardId} not found.");
		}
		if (board.Name == request.NewName)
		{
			throw new InvalidOperationException(
				$"Board with ID {request.BoardId} already has the name '{request.NewName}'.");
		}
		var isNewNameUnique = await boardRepository.IsNameUniqueAsync(request.NewName, request.BoardId);
		if (!isNewNameUnique)
		{
			throw new InvalidOperationException($"Board name '{request.NewName}' is already in use.");
		}
		
		board.RenameBoard(request.NewName);
		await dbcontext.SaveChangesAsync(cancellationToken);
		return board.MapToDto();
	}

	public async Task Handle(RemoveTaskItemToBoardCommand request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);

		var board = await boardRepository.GetByIdAsync(request.BoardId);

		if (board == null)
		{
			throw new KeyNotFoundException($"Board with ID {request.BoardId} not found.");
		}

		if (board.TaskGroup.All(t => t.Id != request.TaskItemId))
		{
			throw new InvalidOperationException($"TaskItem with ID {request.TaskItemId} does not exist in the board.");
		}
		var taskItem = await taskItemRepository.GetByIdAsync(request.TaskItemId);

		if (taskItem == null)
		{
			throw new KeyNotFoundException($"TaskItem with ID {request.TaskItemId} not found.");
		}

		if (taskItem.BoardId != board.Id)
		{
			throw new InvalidOperationException(
				$"TaskItem with ID {request.TaskItemId} does not belong to the board with ID {request.BoardId}.");
		}
		board.RemoveTaskItem(taskItem);
		taskItem.BoardId = 0;
		await dbcontext.SaveChangesAsync(cancellationToken);
	}

	public async Task Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		var board = await boardRepository.GetByIdAsync(request.Id);

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
				await taskItemRepository.UpdateAsync(taskItem);
			}
			board.ClearTaskItems();
		}
		await boardRepository.DeleteAsync(request.Id);
		await dbcontext.SaveChangesAsync(cancellationToken);
	}
}