using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Task4ya.Domain.Repositories;
using Task4ya.Infrastructure.Data;

namespace Task4ya.Application.Services;

public class TaskItemQueueProcessorService : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<TaskItemQueueProcessorService> _logger;
	private readonly TimeSpan _delay = TimeSpan.FromSeconds(5);
	
	public TaskItemQueueProcessorService(IServiceProvider serviceProvider, ILogger<TaskItemQueueProcessorService> logger)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Starting task queue processor");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				using var scope = _serviceProvider.CreateScope();
				var queueService = scope.ServiceProvider.GetRequiredService<IQueueService>();
				var dbContext = scope.ServiceProvider.GetRequiredService<Task4YaDbContext>();
				var taskItemRepository = scope.ServiceProvider.GetRequiredService<ITaskItemRepository>();
				var boardRepository = scope.ServiceProvider.GetRequiredService<IBoardRepository>();
				
				await ProcessTaskItemQueue(
					queueService, 
					dbContext, 
					taskItemRepository, 
					boardRepository,
					stoppingToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing task queue");
			}
			
			await Task.Delay(_delay, stoppingToken);
		}
		
		_logger.LogInformation("Stopping task queue processor");
	}
	
	private async Task ProcessTaskItemQueue(
		IQueueService queueService, 
		Task4YaDbContext dbContext, 
		ITaskItemRepository taskItemRepository, 
		IBoardRepository boardRepository,
		CancellationToken cancellationToken)
	{
		var queueNames = new[]
		{
			"taskitems-add-queue",
			"taskitems-update-queue",
			"taskitems-delete-queue"
		};

		foreach (var queueName in queueNames)
		{
			var queueLength = await queueService.GetQueueLengthAsync(queueName);
			_logger.LogInformation("Processing {QueueLength} tasks from the queue: {Queue}.", queueLength, queueName);
			for (var i = 0; i < queueLength; i++)
			{
				var taskItemFromQueue = await queueService.DequeueAsync<Domain.Entities.TaskItem>(queueName);
				if (taskItemFromQueue == null)
				{
					_logger.LogWarning("Dequeued a null task item.");
					continue;
				}
				
				if (string.IsNullOrWhiteSpace(taskItemFromQueue.Title))
				{
					_logger.LogWarning("Task item with ID {TaskItemId} has an empty title. Skipping.", taskItemFromQueue.Id);
					continue;
				}
			
				try
				{
					switch (queueName)
					{
						case "taskitems-add-queue":
							await AddTaskItemFromQueueToDatabase(taskItemFromQueue, dbContext, taskItemRepository, boardRepository, cancellationToken);
							break;
						case "taskitems-update-queue":
							await UpdateTaskItemFromQueueInDatabase(taskItemFromQueue, dbContext, taskItemRepository, cancellationToken);
							break;
						case "taskitems-delete-queue":
							await DeleteTaskItemFromQueueInDatabase(taskItemFromQueue.Id, dbContext, taskItemRepository, cancellationToken);
							break;
						default:
							_logger.LogWarning("Unknown queue name: {QueueName}", queueName);
							break;
					}
					
					_logger.LogInformation("Processed task item with ID {TaskItemId} from queue {QueueName}.", taskItemFromQueue.Id, queueName);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error processing task item with ID {TaskItemId}.", taskItemFromQueue.Id);
				}
			}
		}
		
	}
	
	private async Task AddTaskItemFromQueueToDatabase(
		Domain.Entities.TaskItem taskItem,
		Task4YaDbContext dbContext,
		ITaskItemRepository taskItemRepository,
		IBoardRepository boardRepository,
		CancellationToken cancellationToken)
	{
		if (await taskItemRepository.GetByIdAsync(taskItem.Id) != null)
		{
			_logger.LogWarning("Task item with ID {TaskItemId} already exists. Skipping.", taskItem.Id);
			return;
		}
		dbContext.TaskItems.Add(taskItem);
		await dbContext.SaveChangesAsync(cancellationToken);
		var board = await boardRepository.GetByIdAsync(taskItem.BoardId);
		board?.AddTaskItem(taskItem);
		await dbContext.SaveChangesAsync(cancellationToken);
		_logger.LogInformation("Successfully added task item with ID {TaskItemId} from queue to database.", taskItem.Id);
	}
	
	private async Task UpdateTaskItemFromQueueInDatabase(
		Domain.Entities.TaskItem taskItem,
		Task4YaDbContext dbContext,
		ITaskItemRepository taskItemRepository,
		CancellationToken cancellationToken)
	{
		var existingTask = await taskItemRepository.GetByIdAsync(taskItem.Id);
		if (existingTask == null)
		{
			_logger.LogWarning("Task item with ID {TaskItemId} does not exist. Cannot update.", taskItem.Id);
			return;
		}
		existingTask.UpdateBoardId(taskItem.BoardId);
		existingTask.UpdateTaskItem(
			taskItem.BoardId,
			taskItem.Title,
			taskItem.Description,
			taskItem.DueDate,
			taskItem.Priority,
			taskItem.Status,
			taskItem.AssigneeToId
		);
		await dbContext.SaveChangesAsync(cancellationToken);
		_logger.LogInformation("Successfully updated task item with ID {TaskItemId} from queue in database.", taskItem.Id);
	}
	
	private async Task DeleteTaskItemFromQueueInDatabase(
		int taskItemId,
		Task4YaDbContext dbContext,
		ITaskItemRepository taskItemRepository,
		CancellationToken cancellationToken)
	{
		var existingTask = await taskItemRepository.GetByIdAsync(taskItemId);
		if (existingTask == null)
		{
			_logger.LogWarning("Task item with ID {TaskItemId} does not exist. Cannot delete.", taskItemId);
			return;
		}
		dbContext.TaskItems.Remove(existingTask);
		await dbContext.SaveChangesAsync(cancellationToken);
		_logger.LogInformation("Successfully deleted task item with ID {TaskItemId} from queue in database.", taskItemId);
	}
}
