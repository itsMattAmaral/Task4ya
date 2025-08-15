using Task4ya.Domain.Utils;

namespace Task4ya.Domain.Entities;

public class User
{
	public int Id { get; init; }
	public string Name { get; set; }
	public string Email { get; set; }
	public string Password { get; set; }
	public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
	public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
	
	public User(string name, string email, string password)
	{
		Name = name;
		Email = email;
		Password = PasswordHandler.HashPassword(password);
	}
	
	public void UpdateUserName(string newName)
	{
		Name = newName;
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void UpdateEmail(string newEmail)
	{
		Email = newEmail;
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void UpdatePassword(string newPassword)
	{
		Password = PasswordHandler.HashPassword(newPassword);
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void UpdateUserDetails(string newName, string newEmail)
	{
		UpdateUserName(newName);
		UpdateEmail(newEmail);
	}
}