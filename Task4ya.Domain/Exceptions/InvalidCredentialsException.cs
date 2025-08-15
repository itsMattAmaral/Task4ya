namespace Task4ya.Domain.Exceptions;

public class InvalidCredentialsException : Exception
{
	public InvalidCredentialsException() 
		: base("Invalid credentials provided.")
	{
	}

	public InvalidCredentialsException(string message) : base(message)
	{
	}
	
	public InvalidCredentialsException(string message, Exception inner) 
		: base(message, inner)
	{
	}
}