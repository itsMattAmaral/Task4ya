using Task4ya.Domain.Utils;

namespace Task4ya.Domain.Entities;

public class Board
{
	public int Id { get; init; }
	public string Name { get; set; }
	public ICollection<TaskItem> TaskGroup { get; init; } = new List<TaskItem>();
	
	public Board(string name = "New Board")
	{
		Name = name;
	}
	
	public void AddTaskItem(TaskItem taskItem)
	{
		TaskGroup.Add(taskItem);
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
			throw new HttpRequestException("TaskItem not found.", null, System.Net.HttpStatusCode.NotFound);
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