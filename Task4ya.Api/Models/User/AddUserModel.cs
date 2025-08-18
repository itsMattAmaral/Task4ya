using System.ComponentModel.DataAnnotations;
using Task4ya.Application.User.Commands.Actions;

namespace Task4ya.Api.Models.User;

public class AddUserModel
{
	[MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
	[MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
	public required string Name { get; set; }
	
	[EmailAddress(ErrorMessage = "Invalid email format.")]
	public required string Email { get; set; }
	
	[Required(ErrorMessage = "Password is required.")]
	[MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
	public required string Password { get; set; }
	
	[DataType(DataType.Text)]
	[Display(Name = "Roles")]
	public required List<string> Roles { get; set; }

	public AddUserCommand GetCommand()
	{
		return new AddUserCommand(Name, Email, Password, Roles);
	}
}