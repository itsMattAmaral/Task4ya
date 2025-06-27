using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.Board.Commands.Actions;

public class AddBoardCommand : IRequest<BoardDto>
{
	public List<int> TaskItemIds{ get; }
	public string Name { get; }
	public AddBoardCommand(List<int> taskItemIds, string name = "New Board")
	{
		TaskItemIds = taskItemIds;
		Name = name;
	}
}