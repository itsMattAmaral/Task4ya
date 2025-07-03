using System.ComponentModel.DataAnnotations;
using Task4ya.Application.TaskItem.Commands.Actions;
using Task4ya.Domain.Enums;

namespace Task4ya.Api.Models.TaskItem;

public class UpdateTaskItemModel
{
	[Required(ErrorMessage = "Board ID is required.")]
	public int BoardId { get; set; }
	
	[MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
	[MinLength(5, ErrorMessage = "Title must be at least 5 characters long.")]
	[Display(Name = "Task Title")]
	[RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Title can only contain letters, numbers, and spaces.")]
	public string? Title { get; set; }
	
	[Display(Name = "Task Description")]
	[MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
	public string? Description { get; set; }
	public DateTime? DueDate { get; set; }
	[Display(Name = "Task Priority")]
	[Range(0, 3)]
	public TaskItemPriority Priority { get; set; }
	[Display(Name = "Task Status")]
	[Range(0, 3)]
	public TaskItemStatus Status { get; set; }

	public UpdateTaskItemCommand GetCommand(int id)
	{
		return new UpdateTaskItemCommand(id, BoardId, Title, Description, DueDate, Priority, Status);
	}
}