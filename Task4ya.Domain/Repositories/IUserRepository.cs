using Task4ya.Domain.Entities;

namespace Task4ya.Domain.Repositories;

public interface IUserRepository
{
	Task<User> GetByIdAsync(int id);
	Task<List<User>> GetAllAsync(int page, int pageSize, string? searchTerm = null, string? sortBy = null, bool sortDescending = false);
	Task<int> GetCountAsync(string? searchTerm = null);
	Task<User?> GetByEmailAsync(string email);
}