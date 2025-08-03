using Microsoft.EntityFrameworkCore;
using Task4ya.Domain.Entities;
using Task4ya.Domain.Repositories;
using Task4ya.Infrastructure.Data;

namespace Task4ya.Infrastructure.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
	private readonly Task4YaDbContext _dbContext;

	public TaskItemRepository(Task4YaDbContext dbContext) => _dbContext = dbContext;
	
	public async Task<TaskItem?> GetByIdAsync(int id)
	{
		return await _dbContext.TaskItems.FindAsync(id);
	}

	public async Task<IEnumerable<TaskItem>> GetAllAsync(int page, int pageSize, string? searchTerm = null, string? sortBy = null, bool sortDescending = false)
	{
		ArgumentNullException.ThrowIfNull(_dbContext);
		ArgumentOutOfRangeException.ThrowIfNegative(page, nameof(page));
		ArgumentOutOfRangeException.ThrowIfNegative(pageSize, nameof(pageSize));
		
		var query = _dbContext.TaskItems.AsQueryable();
		query = query.OrderBy(t => t.Id);
		
		if (!string.IsNullOrEmpty(searchTerm))
		{
			query = query.Where(t => t.Title.Contains(searchTerm));
		}
		
		if (!string.IsNullOrEmpty(sortBy))
		{
			query = sortDescending 
				? query.OrderByDescending(t => EF.Property<object>(t, sortBy)) 
				: query.OrderBy(t => EF.Property<object>(t, sortBy));
		}
		
		return await query 
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();
	}

	public async Task<int> GetCountAsync(string? searchTerm = null)
	{
		var query = _dbContext.TaskItems.AsQueryable();
		
		if (!string.IsNullOrEmpty(searchTerm))
		{
			query = query.Where(t => t.Title.Contains(searchTerm));
		}
		return await query.CountAsync();
	}
	
	public async Task UpdateAsync(TaskItem taskItem)
	{
        ArgumentNullException.ThrowIfNull(taskItem);
        _dbContext.TaskItems.Update(taskItem);
		await _dbContext.SaveChangesAsync();
	}

	public Task<IEnumerable<TaskItem>> GetByBoardIdAsync(int boardId)
	{
		throw new NotImplementedException();
	}
}