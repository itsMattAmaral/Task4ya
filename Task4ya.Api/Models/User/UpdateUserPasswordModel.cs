using System.ComponentModel.DataAnnotations;
using Task4ya.Application.User.Commands.Actions;

namespace Task4ya.Api.Models.User;

public class UpdateUserPasswordModel
{
	[Required(ErrorMessage = "your old Password is required.")]
	public required string OldPassword { get; set; }
	
	[Required(ErrorMessage = "A new Password is required.")]
	[MinLength(6, ErrorMessage = "your new Password must be at least 6 characters long.")]
	public required string NewPassword { get; set; }
	
	public UpdateUserPassword GetCommand()
	{
		return new UpdateUserPassword(OldPassword, NewPassword);
	}
}