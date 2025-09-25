using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Task4ya.Application.Services;

public class RedisCacheService : ICacheService
{
	private readonly IDistributedCache _distributedCache;
	private readonly ILogger<RedisCacheService> _logger;
	private readonly IConnectionMultiplexer _connectionMultiplexer;

	public RedisCacheService(IDistributedCache distributedCache, ILogger<RedisCacheService> logger, IConnectionMultiplexer connectionMultiplexer)
	{
		_distributedCache = distributedCache;
		_logger = logger;
		_connectionMultiplexer = connectionMultiplexer;
	}

	public async Task<T?> GetAsync<T>(string key) where T : class
	{
		try
		{
			var cachedValue = await _distributedCache.GetStringAsync(key);
			if (string.IsNullOrEmpty(cachedValue)) return null;

			return JsonSerializer.Deserialize<T>(cachedValue);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting cache for key {Key}", key);
			return null;
		}
	}

	public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
	{
		try
		{
			var options = new DistributedCacheEntryOptions();

			if (expiration.HasValue)
			{
				options.SetAbsoluteExpiration(expiration.Value);
			}
			var serializedValue = JsonSerializer.Serialize(value);
			await _distributedCache.SetStringAsync(key, serializedValue, options);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error setting cache for key {Key}", key);
		}
	}

	public async Task RemoveAsync(string key)
	{
		try
		{
			await _distributedCache.RemoveAsync(key);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error removing cache for key {Key}", key);
		}
	}
	
	public async Task RemoveByPatternAsync(string pattern)
	{
		pattern = $"*_{pattern}:*";
		try
		{
			var database = _connectionMultiplexer.GetDatabase();
			var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
			
			var keys = server.Keys(pattern: pattern); // Change to keysAsync when going to production
			
			var keyArray = keys.ToArray();
			if (keyArray.Length > 0)
			{
				// Delete all matching keys in a single batch operation
				await database.KeyDeleteAsync(keyArray);
				_logger.LogInformation("Removed {Count} keys matching pattern {Pattern}", keyArray.Length, pattern);
			}
			else
			{
				_logger.LogInformation("No keys found matching pattern {Pattern}", pattern);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error removing cache keys by pattern {Pattern}", pattern);
		}
	}
}
