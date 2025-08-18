using Task4ya.Application.Dtos;

namespace Task4ya.Application.Mappers;

public static class UserMapper
{
	public static UserDto MapToDto(this Domain.Entities.User entity)
	{
		ArgumentNullException.ThrowIfNull(entity);

		return new UserDto
		{
			Id = entity.Id,
			Name = entity.Name,
			Email = entity.Email,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt,
			Roles = entity.Roles.Select(role => role).ToList()
		};
	}
}