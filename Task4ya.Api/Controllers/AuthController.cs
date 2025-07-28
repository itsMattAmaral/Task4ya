using System.Net;
using Microsoft.AspNetCore.Mvc;
using Task4ya.Api.Helpers;
using Task4ya.Api.Models;

namespace Task4ya.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly AuthHelpers _authHelpers;
	public AuthController(AuthHelpers authHelpers)
	{
		_authHelpers = authHelpers ?? throw new ArgumentNullException(nameof(authHelpers));
	}
	
	[HttpPost("login")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	public IActionResult Login([FromBody] LoginModel model)
	{
		if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
		{
			return BadRequest("Invalid login request.");
		}
		var user = _authHelpers.GetUserByEmailAsync(model.Email).Result;
		if (user == null)
		{
			return Unauthorized("Invalid email or password.");
		}
		if (user.Password != model.Password)
		{
			return Unauthorized("Invalid email or password.");
		}
		
		var token = _authHelpers.GenerateJwtToken(user);
		
		return Ok(new {token});
	}
	
}