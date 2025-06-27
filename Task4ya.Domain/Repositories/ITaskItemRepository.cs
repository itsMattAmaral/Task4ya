using Task4ya.Domain.Entities;

namespace Task4ya.Domain.Repositories;

public interface ITaskItemRepository
{
	Task<TaskItem?> GetByIdAsync(int id);
	Task<IEnumerable<TaskItem>> GetAllAsync();
	Task<IEnumerable<TaskItem>> GetByBoardIdAsync(int boardId);
}