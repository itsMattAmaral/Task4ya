namespace Task4ya.Domain.Utils;

public class PasswordHandler
{
	public static string HashPassword(string password)
	{
		if (string.IsNullOrEmpty(password))
			throw new ArgumentException("Password cannot be null or empty.", nameof(password));

		return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
	}
	
	public static bool VerifyPassword(string password, string hashedPassword)
	{
		if (string.IsNullOrEmpty(password))
			throw new ArgumentException("Password cannot be null or empty.", nameof(password));
		if (string.IsNullOrEmpty(hashedPassword))
			throw new ArgumentException("Hashed password cannot be null or empty.", nameof(hashedPassword));

		return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
	}
}