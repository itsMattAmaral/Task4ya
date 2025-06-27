using Task4ya.Domain.Repositories;
using Task4ya.Domain.Utils;

namespace Task4ya.Domain.Entities;

public class Board
{
	public int Id { get; set; }
	public string Name { get; set; }
	public ICollection<TaskItem> TaskGroup { get; set; } = new List<TaskItem>();
	
	public Board(int taskItemId, string name = "New Board")
	{
		Name = name;
	}
	
	public Board(string name = "New Board")
	{
		Name = name;
		TaskGroup = new List<TaskItem>();
	}
	
	public Board()
	{
		Name = "New Board";
		TaskGroup = new List<TaskItem>();
	}
	
	public async Task<bool> AddTaskItem(int taskItemId, ITaskItemRepository repository)
	{
		var taskItem = await repository.GetByIdAsync(taskItemId);
		if (taskItem is null) return false;
		if (TaskGroup.Any(t => t.Id == taskItem.Id)) return false;
		TaskGroup.Add(taskItem);
		return true;
	}
	
	public void RemoveTaskItem(int taskItemId)
	{
		var taskItem = TaskGroup.FirstOrDefault(t => t.Id == taskItemId);
		if (taskItem is not null)
		{
			TaskGroup.Remove(taskItem);
		}
		else
		{
			throw new KeyNotFoundException($"TaskItem with ID {taskItemId} not found in the board.");
		}
	}
	
	public void ClearTaskItems()
	{
		TaskGroup.Clear();
	}
	
	public void RenameBoard(string newName)
	{
		StringValidator.ThrowIfNullOrWhiteSpace(newName, nameof(newName));
		Name = newName;
	}
}