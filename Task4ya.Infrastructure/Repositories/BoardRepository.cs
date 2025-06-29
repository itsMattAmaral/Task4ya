using Microsoft.EntityFrameworkCore;
using Task4ya.Domain.Entities;
using Task4ya.Domain.Repositories;
using Task4ya.Infrastructure.Data;

namespace Task4ya.Infrastructure.Repositories;

public class BoardRepository : IBoardRepository
{
	private readonly Task4YaDbContext _dbContext;
	
	public BoardRepository(Task4YaDbContext dbContext)
	{
		_dbContext = dbContext;
	}
	
	public async Task<Board> GetByIdAsync(int id)
	{
		return await _dbContext.Boards.FindAsync(id) 
			?? throw new KeyNotFoundException($"Board with ID {id} not found.");
	}

	public async Task<IEnumerable<Board>> GetAllAsync(int page, int pageSize, string? searchTerm = null, string? sortBy = null, bool sortDescending = false)
	{
		var query = _dbContext.Boards.AsQueryable();

		if (!string.IsNullOrEmpty(searchTerm))
		{
			query = query.Where(b => b.Name.Contains(searchTerm));
		}

		if (!string.IsNullOrEmpty(sortBy))
		{
			query = sortDescending
				? query.OrderByDescending(b => EF.Property<object>(b, sortBy))
				: query.OrderBy(b => EF.Property<object>(b, sortBy));
		}

		return await query
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.Include(b => b.TaskGroup) 
			.ToListAsync();
	}
	
	public async Task<int> GetCountAsync(string? searchTerm = null)
	{
		var query = _dbContext.Boards.AsQueryable();
    
		if (!string.IsNullOrEmpty(searchTerm))
		{
			query = query.Where(b => b.Name.Contains(searchTerm));
		}
    
		return await query.CountAsync();
	}
}