using System.ComponentModel.DataAnnotations;
using Task4ya.Application.User.Commands.Actions;

namespace Task4ya.Api.Models.User;

public class UpdateUserModel
{
	[MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
	[MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
	public required string Name { get; set; }
	
	[EmailAddress(ErrorMessage = "Invalid email format.")]
	public required string Email { get; set; }
	
	public UpdateUserCommand GetCommand()
	{
		return new UpdateUserCommand(Name, Email);
	}
}