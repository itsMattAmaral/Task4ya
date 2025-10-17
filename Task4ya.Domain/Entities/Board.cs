using Task4ya.Domain.Utils;

namespace Task4ya.Domain.Entities;

public class Board(int id, int ownerId, string name = "New Board")
{
	public int Id { get; init; } = id;
	public string Name { get; set; } = name;
	public int OwnerId {get; set;} = ownerId;
	public ICollection<TaskItem> TaskGroup { get; init; } = new List<TaskItem>();
	
	public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
	public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

	public void AddTaskItem(TaskItem taskItem)
	{
		TaskGroup.Add(taskItem);
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void RemoveTaskItem(TaskItem taskItem)
	{
		TaskGroup.Remove(taskItem);
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void ClearTaskItems()
	{
		TaskGroup.Clear();
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void RenameBoard(string newName)
	{
		StringValidator.ThrowIfNullOrWhiteSpace(newName, nameof(newName));
		Name = newName;
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void ChangeOwner(int newOwnerId)
	{
		OwnerId = newOwnerId;
		UpdatedAt = DateTime.UtcNow;
	}
}