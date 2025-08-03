using Task4ya.Domain.Entities;

namespace Task4ya.Domain.Repositories;

public interface ITaskItemRepository
{
	Task<TaskItem?> GetByIdAsync(int id);
	Task<IEnumerable<TaskItem>> GetAllAsync(int page, int pageSize, string? searchTerm = null, string? sortBy = null, bool sortDescending = false);
	Task UpdateAsync(TaskItem taskItem);
	Task<IEnumerable<TaskItem>> GetByBoardIdAsync(int boardId);
	
	Task<int> GetCountAsync(string? searchTerm = null);
}