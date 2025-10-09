using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Task4ya.Domain.Repositories;
using Task4ya.Infrastructure.Data;

namespace Task4ya.Application.Services;

public class BoardsQueueProcessorService : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<BoardsQueueProcessorService> _logger;
	private readonly TimeSpan _delay = TimeSpan.FromSeconds(5);
	
	public BoardsQueueProcessorService(IServiceProvider serviceProvider, ILogger<BoardsQueueProcessorService> logger)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
	}
	
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Starting boards queue processor");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				using var scope = _serviceProvider.CreateScope();
				var queueService = scope.ServiceProvider.GetRequiredService<IQueueService>();
				var dbContext = scope.ServiceProvider.GetRequiredService<Task4YaDbContext>();
				var boardRepository = scope.ServiceProvider.GetRequiredService<IBoardRepository>();
				
				await ProcessBoardsQueue(
					queueService, 
					dbContext, 
					boardRepository,
					stoppingToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing boards queue");
			}
			
			await Task.Delay(_delay, stoppingToken);
		}
		
		_logger.LogInformation("Stopping boards queue processor");
	}
	
	private async Task ProcessBoardsQueue(
		IQueueService queueService, 
		Task4YaDbContext dbContext, 
		IBoardRepository boardRepository,
		CancellationToken cancellationToken)
	{
		var queueNames = new[]
		{
			"boards-add-queue",
			"boards-update-queue",
			"boards-delete-queue"
		};

		foreach (var queueName in queueNames)
		{
			var queueLength = await queueService.GetQueueLengthAsync(queueName);
			if (queueLength <= 0) continue;

			_logger.LogInformation("Processing {QueueLength} items from {QueueName}", queueLength, queueName);
				
			for (var i = 0; i < queueLength; i++)
			{
				var boardFromQueue = await queueService.DequeueAsync<Domain.Entities.Board>(queueName);

				if (boardFromQueue is null)
				{
					_logger.LogWarning("{QueueName} queue doesn't exist", queueName);
				}

				try
				{
					switch (queueName)
					{
						case "boards-add-queue":
							if (boardFromQueue is not null)
							{
								await AddBoardFromQueueToDatabase(boardFromQueue, dbContext, boardRepository, cancellationToken);
							}
							break;
						case "boards-update-queue":
							if (boardFromQueue is not null)
							{
								await UpdateBoardFromQueueInDatabase(boardFromQueue, dbContext, boardRepository, cancellationToken);
							}
							break;
						case "boards-delete-queue":
							if (boardFromQueue is not null)
							{
								await DeleteBoardFromQueueInDatabase(boardFromQueue.Id, dbContext, boardRepository, cancellationToken);
							}
							break;
						default:
							_logger.LogWarning("Unknown queue name: {QueueName}", queueName);
							break;
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error processing board from {QueueName}", queueName);
				}
			}


		}
	}
	
	private async Task AddBoardFromQueueToDatabase(
		Domain.Entities.Board boardFromQueue, 
		Task4YaDbContext dbContext, 
		IBoardRepository boardRepository,
		CancellationToken cancellationToken)
	{
		var existingBoard = await boardRepository.GetByIdAsync(boardFromQueue.Id);
		if (existingBoard is not null)
		{
			_logger.LogWarning("Board with ID {BoardId} already exists. Skipping add.", boardFromQueue.Id);
			return;
		}

		dbContext.Boards.Add(boardFromQueue);
		await dbContext.SaveChangesAsync(cancellationToken);
		_logger.LogInformation("Added board with ID {BoardId} to the database.", boardFromQueue.Id);
	}
	
	private async Task UpdateBoardFromQueueInDatabase(
		Domain.Entities.Board boardFromQueue, 
		Task4YaDbContext dbContext, 
		IBoardRepository boardRepository,
		CancellationToken cancellationToken)
	{
		var existingBoard = await boardRepository.GetByIdAsync(boardFromQueue.Id);
		if (existingBoard is null)
		{
			_logger.LogWarning("Board with ID {BoardId} does not exist. Skipping update.", boardFromQueue.Id);
			return;
		}

		existingBoard.RenameBoard(boardFromQueue.Name);
		if (existingBoard.OwnerId != boardFromQueue.OwnerId) existingBoard.ChangeOwner(boardFromQueue.OwnerId);
		
		var incomingTaskIds = boardFromQueue.TaskGroup.Select(t => t.Id).ToHashSet();
		
		foreach (var taskItem in existingBoard.TaskGroup.ToList().Where(taskItem => !incomingTaskIds.Contains(taskItem.Id)))
		{
			existingBoard.RemoveTaskItem(taskItem);
		}
		
		foreach (var taskItem in boardFromQueue.TaskGroup)
		{
			var existingTask = existingBoard.TaskGroup.FirstOrDefault(t => t.Id == taskItem.Id);
    
			if (existingTask != null)
			{
				dbContext.Entry(existingTask).CurrentValues.SetValues(taskItem);
			}
			else
			{
				var trackedTask = dbContext.TaskItems.Local.FirstOrDefault(t => t.Id == taskItem.Id);
        
				if (trackedTask != null)
				{
					dbContext.Entry(trackedTask).CurrentValues.SetValues(taskItem);
					existingBoard.AddTaskItem(trackedTask);
				}
				else
				{
					dbContext.TaskItems.Update(taskItem);
					existingBoard.AddTaskItem(taskItem);
				}
			}
		}
		await dbContext.SaveChangesAsync(cancellationToken);
		_logger.LogInformation("Updated board with ID {BoardId} in the database.", boardFromQueue.Id);
	}
	
	private async Task DeleteBoardFromQueueInDatabase(
		int boardId, 
		Task4YaDbContext dbContext, 
		IBoardRepository boardRepository,
		CancellationToken cancellationToken)
	{
		var existingBoard = await boardRepository.GetByIdAsync(boardId);
		if (existingBoard is null)
		{
			_logger.LogWarning("Board with ID {BoardId} does not exist. Skipping delete.", boardId);
			return;
		}

		await boardRepository.DeleteAsync(existingBoard.Id);
		await dbContext.SaveChangesAsync(cancellationToken);
		_logger.LogInformation("Deleted board with ID {BoardId} from the database.", boardId);
	}
}