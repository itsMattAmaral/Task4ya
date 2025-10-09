using MediatR;
using Task4ya.Application.Board.Commands.Actions;
using Task4ya.Application.Common.Commands;
using Task4ya.Application.Helpers;
using Task4ya.Application.Dtos;
using Task4ya.Application.Mappers;
using Task4ya.Application.Services;
using Task4ya.Domain.Repositories;

namespace Task4ya.Application.Board.Commands;

public class BoardCommandHandler(
	ITaskItemRepository taskItemRepository,
	IUserRepository userRepository,
	IBoardRepository boardRepository,
	IQueueService queueService,
	IMediator mediator)
	:
		IRequestHandler<AddBoardCommand, BoardDto>,
		IRequestHandler<DeleteBoardCommand>,
		IRequestHandler<AddTaskItemToBoardCommand>,
		IRequestHandler<UpdateBoardNameCommand, BoardDto>,
		IRequestHandler<UpdateBoardOwnerCommand>,
		IRequestHandler<RemoveTaskItemToBoardCommand>
{
	public async Task<BoardDto> Handle(AddBoardCommand request, CancellationToken cancellationToken)
	{
		var owner = await userRepository.GetByIdAsync(request.OwnerId);
		if (owner == null)
		{
			throw new KeyNotFoundException($"User with ID {request.OwnerId} not found.");
		}
		var newBoard = new Domain.Entities.Board(request.OwnerId, request.Name);

		if (request.TaskItemIds.Count > 0)
		{
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
				await queueService.EnqueueAsync("taskitems-update-queue", taskItem);
				newBoard.AddTaskItem(taskItem);
			}
		}

		await queueService.EnqueueAsync("boards-add-queue", newBoard);
		await InvalidateCachesAsync(
			[],
			[CacheKeyGenerator.BoardsPrefix], 
			cancellationToken);
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
		await queueService.EnqueueAsync("taskitems-update-queue", taskItem);
		
		board.AddTaskItem(taskItem);
		await queueService.EnqueueAsync("boards-update-queue", board);
		
		await InvalidateCachesAsync(
			[],
			[CacheKeyGenerator.BoardsPrefix], 
			cancellationToken);
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
		await queueService.EnqueueAsync("boards-update-queue", board);
		await InvalidateCachesAsync(
			[],
			[CacheKeyGenerator.BoardsPrefix], 
			cancellationToken);
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
		await queueService.EnqueueAsync("boards-update-queue", board);
		
		taskItem.BoardId = null;
		await queueService.EnqueueAsync("taskitems-update-queue", board);
		
		await InvalidateCachesAsync(
			[],
			[CacheKeyGenerator.BoardsPrefix], 
			cancellationToken);
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
				taskItem.BoardId = null;
				await queueService.EnqueueAsync("taskitems-update-queue", taskItem);
			}
			board.ClearTaskItems();
		}
		await queueService.EnqueueAsync("boards-delete-queue", board);
		await InvalidateCachesAsync(
			[],
			[CacheKeyGenerator.BoardsPrefix], 
			cancellationToken);
	}
	
	public async Task Handle(UpdateBoardOwnerCommand request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		var board = await boardRepository.GetByIdAsync(request.BoardId);
		if (board == null)
		{
			throw new KeyNotFoundException($"Board with ID {request.BoardId} not found.");
		}
		if (board.OwnerId == request.NewOwnerId) 
		{
			throw new InvalidOperationException(
				$"Board with ID {request.BoardId} already has the owner with ID {request.NewOwnerId}.");
		}
		var newOwner = await userRepository.GetByIdAsync(request.NewOwnerId);
		if (newOwner == null)
		{
			throw new KeyNotFoundException($"User with ID {request.NewOwnerId} not found.");
		}
		board.ChangeOwner(request.NewOwnerId);
		await queueService.EnqueueAsync("boards-update-queue", board);
		await InvalidateCachesAsync(
			[],
			[CacheKeyGenerator.BoardsPrefix], 
			cancellationToken);
	}

	private async Task InvalidateCachesAsync(string[] keys, string[] patterns, CancellationToken cancellationToken)
	{
		await mediator.Send(new InvalidateCacheCommand { Keys = keys, Patterns = patterns }, cancellationToken);
	}
}