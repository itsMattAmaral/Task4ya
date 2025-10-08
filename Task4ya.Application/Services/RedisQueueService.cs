using StackExchange.Redis;

namespace Task4ya.Application.Services;

public class RedisQueueService(IConnectionMultiplexer connectionMultiplexer)
	: IQueueService
{
	private readonly IDatabase _database = connectionMultiplexer.GetDatabase();

	public async Task EnqueueAsync<T>(string queueName, T? item) where T : class
	{
		await _database.ListRightPushAsync(queueName, System.Text.Json.JsonSerializer.Serialize(item));
	}
	
	public async Task<T?> DequeueAsync<T>(string queueName) where T : class
	{
		await Task.Delay(100); // Simulate some delay 
		var value = await _database.ListLeftPopAsync(queueName);
		return value.IsNullOrEmpty ? null : System.Text.Json.JsonSerializer.Deserialize<T>(value!);
	}
	
	public async Task<long> GetQueueLengthAsync(string queueName)
	{
		return await _database.ListLengthAsync(queueName);
	}
	
	public async Task ClearQueueAsync(string queueName)
	{
		await _database.KeyDeleteAsync(queueName);
	}
	
	public async Task BlockingDequeueAsync(string queueName)
	{
		await _database.ListLeftPopAsync(queueName);
	}
}