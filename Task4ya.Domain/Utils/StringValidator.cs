namespace Task4ya.Domain.Utils;

public static class StringValidator
{
	public static void ThrowIfNullOrWhiteSpace(string? value, string fieldName)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new ArgumentException($"{fieldName} cannot be empty.", nameof(value));
		}
	}
}