using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Task4ya.Domain.Entities;
using Task4ya.Domain.Repositories;

namespace Task4ya.Api.Helpers;

public class AuthHelpers(IUserRepository userRepository, IConfiguration configuration)
{
	private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
	private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

	public string GenerateJwtToken(User? user)
	{
		if (user == null)
		{
			throw new ArgumentNullException(nameof(user), "User cannot be null.");
		}
		
		var claims = new List<Claim>
		{
			new (ClaimTypes.NameIdentifier, user.Id.ToString()),
			new (ClaimTypes.Name, user.Name),
			new (ClaimTypes.Email, user.Email)
			
		};
		claims.AddRange(user.Roles.Select(role => role.ToString()).Select(roleName => new Claim(ClaimTypes.Role, roleName)));

		var key = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(_configuration["ApplicationSettings:JWT_Secret"] ?? string.Empty));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
		
		var jwtToken = new JwtSecurityToken
			(
			claims: claims,
			notBefore: DateTime.UtcNow,
			expires: DateTime.UtcNow.AddDays(30),
			signingCredentials: credentials
			);
		return new JwtSecurityTokenHandler().WriteToken(jwtToken);
	}
	
	public async Task<User?> GetUserByEmailAsync(string email)
	{
		if (string.IsNullOrEmpty(email))
		{
			throw new ArgumentException("Email cannot be null or empty.", nameof(email));
		}
		
		return await _userRepository.GetByEmailAsync(email);
	}
}