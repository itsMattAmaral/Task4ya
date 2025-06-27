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
		return await _dbContext.TaskItems.FindAsync(id) 
			?? throw new KeyNotFoundException($"TaskItem with ID {id} not found.");
	}

	public async Task<IEnumerable<TaskItem>> GetAllAsync()
	{
		return await _dbContext.TaskItems.ToListAsync();
	}

	public Task<IEnumerable<TaskItem>> GetByBoardIdAsync(int boardId)
	{
		throw new NotImplementedException();
	}
}