using MediatR;
using Task4ya.Application.Board.Commands.Actions;
using Task4ya.Application.Dtos;
using Task4ya.Application.Mappers;
using Task4ya.Domain.Repositories;
using Task4ya.Infrastructure.Data;

namespace Task4ya.Application.Board.Commands;

public class BoardCommandHandler :
	IRequestHandler<AddBoardCommand, BoardDto>,
	IRequestHandler<DeleteBoardCommand>
{
	private readonly Task4YaDbContext _dbcontext;
	private readonly ITaskItemRepository _taskItemRepository;
	private readonly IBoardRepository _boardRepository;

	public BoardCommandHandler(Task4YaDbContext dbcontext, ITaskItemRepository taskItemRepository,
		IBoardRepository boardRepository)
	{
		_dbcontext = dbcontext;
		_taskItemRepository = taskItemRepository;
		_boardRepository = boardRepository;
	}

	public async Task<BoardDto> Handle(AddBoardCommand request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);

		var newBoard = new Domain.Entities.Board(request.Name);

		foreach (var taskId in request.TaskItemIds)
		{
			await newBoard.AddTaskItem(taskId, _taskItemRepository);
		}
		_dbcontext.Add(newBoard);
		await _dbcontext.SaveChangesAsync(cancellationToken);
		return newBoard.MapToDto();
	}

	public async Task Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);
		await _boardRepository.DeleteAsync(request.Id);
	}
}