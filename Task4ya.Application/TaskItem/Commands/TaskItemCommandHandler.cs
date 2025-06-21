using MediatR;
using Task4ya.Application.Dtos;
using Task4ya.Application.TaskItem.Commands.Actions;
using Task4ya.Infrastructure.Data;

namespace Task4ya.Application.TaskItem.Commands;

public class TaskItemCommandHandler : IRequestHandler<AddTaskItemCommand, TaskItemDto>
{
	private readonly Task4YaDbContext _dbcontext;
	public TaskItemCommandHandler(Task4YaDbContext dbcontext)
		{
			_dbcontext = dbcontext;
		}
	
	public async Task<TaskItemDto> Handle(AddTaskItemCommand request, CancellationToken cancellationToken)
	{
		var newTask = new Domain.Entities.TaskItem(
			request.Title,
			request.Description,
			request.DueDate
		);
		_dbcontext.Add(newTask);
		await _dbcontext.SaveChangesAsync(cancellationToken);
		return new TaskItemDto
		{
			Id = newTask.Id,
			Title = newTask.Title,
			Description = newTask.Description,
			DueDate = newTask.DueDate,
			Status = newTask.Status,
			Priority = newTask.Priority,
			CreatedAt = newTask.CreatedAt,
			UpdatedAt = newTask.UpdatedAt
		};
	}
}