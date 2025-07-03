using System.ComponentModel.DataAnnotations;
using Task4ya.Application.TaskItem.Commands.Actions;
using Task4ya.Domain.Enums;

namespace Task4ya.Api.Models.TaskItem;

public class AddTaskItemModel
{
	[Required(ErrorMessage = "Board ID is required.")]
	public int BoardId { get; set; }
	
	[Required(ErrorMessage = "Title is required.")]
	[MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
	[MinLength(5, ErrorMessage = "Title must be at least 5 characters long.")]
	[Display(Name = "Task Title")]
	[RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Title can only contain letters, numbers, and spaces.")]
	public string Title { get; set; } = string.Empty;
	
	[MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
	[Display(Name = "Task Description")]
	public string? Description { get; set; }
	
	public DateTime? DueDate { get; set; }
	[Display(Name = "Task Priority")]
	[Range(0, 3)]
	public TaskItemPriority Priority { get; set; } = TaskItemPriority.Medium;
	[Display(Name = "Task Status")]
	[Range(0, 3)]
	public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;

	public AddTaskItemCommand GetCommand()
	{
		return new AddTaskItemCommand(BoardId, Title, Description, DueDate, Priority, Status);
	}
}