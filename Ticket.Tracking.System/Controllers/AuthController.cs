using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Ticket.Tracking.System.Configurations;
using Ticket.Tracking.System.Data;
using Ticket.Tracking.System.Models;
using Ticket.Tracking.System.Models.DTOS;

namespace Ticket.Tracking.System.Controllers;
[Route("api/[controll]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly AppDbContext _context;
	private readonly UserManager<IdentityUser> _userManager;
	private readonly JwtConfig _jwtConfig;


	public AuthController(
		AppDbContext context,
		UserManager<IdentityUser> userManager,
		JwtConfig jwtConfig)
	{
		_context = context;
		_userManager = userManager;
		_jwtConfig = jwtConfig;
	}

	[HttpPost]
	public async Task<IActionResult> Register([FromBody] UserRegisterationRequestDto user)
	{
		if (ModelState.IsValid)
		{
			var user_exist = await _userManager.FindByEmailAsync(user.Email);
			if(user_exist != null)
			{
				return BadRequest(new AuthResult()
				{
					Errors = new List<string>()
					{
						"Email already exist"
					},
					Result = false,
					Token = null,
				});
			}
			var new_user = new IdentityUser()
			{
				Email = user.Email,
				UserName = user.Name,
			};
			var isCreated = await _userManager.CreateAsync(new_user, user.Password);
			if(isCreated.Succeeded)
			{

			}

			return BadRequest(new AuthResult()
			{
				Errors = new List<string>()
				{
					"Sever Error"
				},
				Result = false,
			});
		}
		return BadRequest();
	}


	private string GenerateJwtToken(IdentityUser user)
	{
		var jwtTokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret!);
	}
}
