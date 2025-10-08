using Task4ya.Domain.Entities;

namespace Task4ya.Domain.Repositories;

public interface IBoardRepository
{
	Task<Board?> GetByIdAsync(int? id);
	Task<IEnumerable<Board>> GetAllAsync(int page, int pageSize, string? searchTerm = null, string? sortBy = null, bool sortDescending = false);
	Task<int> GetCountAsync(string? searchTerm = null);
	Task<bool> IsNameUniqueAsync(string newName, int? boardId = null);
	Task DeleteAsync(int id);
}