using System.ComponentModel.DataAnnotations;

namespace Task4ya.Api.Models;

public class LoginModel
{
	[EmailAddress(ErrorMessage = "Invalid email format.")]
	public required string Email { get; set; }
	
	[Required(ErrorMessage = "Password is required.")]
	public required string Password { get; set; }

}