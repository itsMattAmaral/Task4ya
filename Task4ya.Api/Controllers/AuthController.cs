using System.Net;
using Microsoft.AspNetCore.Mvc;
using Task4ya.Api.Helpers;
using Task4ya.Api.Models;
using Task4ya.Domain.Exceptions;
using Task4ya.Domain.Utils;

namespace Task4ya.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthHelpers authHelpers) : ControllerBase
{
	private readonly AuthHelpers _authHelpers = authHelpers ?? throw new ArgumentNullException(nameof(authHelpers));

	[HttpPost("login")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public async Task<IActionResult> Login([FromBody] LoginModel model)
	{
		if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
		{
			return BadRequest("Invalid login request.");
		}
		var user = await _authHelpers.GetUserByEmailAsync(model.Email);
		if (user == null) throw new UserNotFoundException($"User with email {model.Email} not found.");
		var isPasswordValid = PasswordHandler.VerifyPassword(model.Password, user.Password);

		if (!isPasswordValid)
		{
			return BadRequest("Invalid email or password.");
		}

		var token = _authHelpers.GenerateJwtToken(user);
		
		return Ok(new {token});
	}
	
}