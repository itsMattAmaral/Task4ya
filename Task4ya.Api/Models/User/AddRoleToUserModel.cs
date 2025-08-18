using Task4ya.Application.User.Commands.Actions;

namespace Task4ya.Api.Models.User;

public class AddRoleToUserModel
{
	public string Role { get; set; } = string.Empty;

	public AddRoleToUserCommand GetCommand()
	{
		return new AddRoleToUserCommand(Role);
	}
}