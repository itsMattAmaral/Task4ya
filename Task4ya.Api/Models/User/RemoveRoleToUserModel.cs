using Task4ya.Application.User.Commands.Actions;

namespace Task4ya.Api.Models.User;

public class RemoveRoleToUserModel
{
	public string Role { get; set; } = string.Empty;

	public RemoveRoleToUserCommand GetCommand()
	{
		return new RemoveRoleToUserCommand(Role);
	}
}