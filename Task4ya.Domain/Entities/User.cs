using Task4ya.Domain.Enums;

namespace Task4ya.Domain.Entities;

public class User(string name, string email, string password)
{
	public int Id { get; init; }
	public string Name { get; set; } = name;
	public string Email { get; set; } = email.ToLower();
	public string Password { get; set; } = password;
	public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
	public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
	
	public List<Roles> Roles { get; init; } = [Enums.Roles.User];

	public void UpdateUserName(string newName)
	{
		Name = newName;
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void UpdateEmail(string newEmail)
	{
		Email = newEmail.ToLower();
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void UpdatePassword(string newPassword)
	{
		Password = newPassword;
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void UpdateUserDetails(string newName, string newEmail)
	{
		UpdateUserName(newName);
		UpdateEmail(newEmail);
	}
	
	public void AddRole(Roles role)
	{
		if (Roles.Contains(role)) return;

		Roles.Add(role);
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void RemoveRole(Roles role)
	{
		if (!Roles.Contains(role)) return;

		Roles.Remove(role);
		UpdatedAt = DateTime.UtcNow;
	}
}