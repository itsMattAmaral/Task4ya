using Microsoft.EntityFrameworkCore;
using Task4ya.Domain.Entities;
using Task4ya.Domain.Repositories;
using Task4ya.Infrastructure.Data;

namespace Task4ya.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
	private readonly Task4YaDbContext _dbContext;
	
	public UserRepository(Task4YaDbContext dbContext)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}
	
	public async Task<User> GetByIdAsync(int id)
	{
		return await _dbContext.Users
			.FirstOrDefaultAsync(u => u.Id == id) 
			?? throw new KeyNotFoundException($"User with ID {id} not found.");
	}

	public async Task<List<User>> GetAllAsync(int page, int pageSize, string? searchTerm = null, string? sortBy = null, bool sortDescending = false)
	{
		var query = _dbContext.Users.AsQueryable();
		query = query.OrderBy(u => u.Id);

		if (!string.IsNullOrEmpty(searchTerm))
		{
			query = query.Where(u => u.Name.Contains(searchTerm) || u.Email.Contains(searchTerm));
		}

		if (!string.IsNullOrEmpty(sortBy))
		{
			query = sortDescending
				? query.OrderByDescending(u => EF.Property<object>(u, sortBy))
				: query.OrderBy(u => EF.Property<object>(u, sortBy));
		}

		return await query
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();
	}

	public async Task<int> GetCountAsync(string? searchTerm = null)
	{
		var query = _dbContext.Users.AsQueryable();
		
		if (!string.IsNullOrEmpty(searchTerm))
		{
			query = query.Where(u => u.Name.Contains(searchTerm) || u.Email.Contains(searchTerm));
		}
		
		return await query.CountAsync();
	}

	public async Task<User?> GetByEmailAsync(string email)
	{
		return await _dbContext.Users
			.FirstOrDefaultAsync(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
	}
	
	public async Task<bool> ExistsAsync(string newUserEmail, CancellationToken cancellationToken)
	{
		return await _dbContext.Users
			.AnyAsync(u => u.Email.Equals(newUserEmail, StringComparison.OrdinalIgnoreCase), cancellationToken);
	}
}