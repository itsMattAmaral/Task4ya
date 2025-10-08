namespace Task4ya.Application.Services;

public interface IQueueService
{
	public Task EnqueueAsync<T>(string queueName, T? item) where T : class;
	
	public Task<T?> DequeueAsync<T>(string queueName) where T : class;
	
	public Task<long> GetQueueLengthAsync(string queueName);
	
	public Task ClearQueueAsync(string queueName);
	
	public Task BlockingDequeueAsync(string queueName);
	
}