using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ticket.Tracking.System.Configurations;
using Ticket.Tracking.System.Data;
using Ticket.Tracking.System.Models;
using Ticket.Tracking.System.Models.DTOS;

namespace Ticket.Tracking.System.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly AppDbContext _context;
	private readonly UserManager<IdentityUser> _userManager;
	// private readonly JwtConfig _jwtConfig;
	private readonly IConfiguration _configuration;
  private readonly RoleManager<IdentityRole> _roleManager;
  private readonly ILogger<AuthController> _logger;

  public AuthController(
		AppDbContext context,
		UserManager<IdentityUser> userManager,
		IConfiguration configuration,
		RoleManager<IdentityRole> roleManager,
		ILogger<AuthController> logger)
	{
		_context = context;
		_userManager = userManager;
		_configuration = configuration;
		_roleManager = roleManager;
		_logger = logger;
	}

	[HttpPost]
	[Route("register")]
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
				await _userManager.AddToRoleAsync(new_user, user.RoleName);
				var token = await GenerateJwtToken(new_user);
				return Ok(new AuthResult(){
					Result = true,
					Token = token,
				});
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

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] UserLoginRequestDto user){
		if(ModelState.IsValid){
			var existing_user = await _userManager.FindByEmailAsync(user.Email);
			if(existing_user == null){
				return BadRequest(new AuthResult(){
					Errors = new List<string>(){
						"Invalid Credentials"
					},
					Result = false,
				});
			}

			var is_correct = await _userManager.CheckPasswordAsync(existing_user, user.Password);
			if(is_correct){
				var jwtToken = await GenerateJwtToken(existing_user);
				return Ok(new AuthResult(){
					Token = jwtToken,
					Result = true,
				});
			}
			return BadRequest(new AuthResult(){
				Errors = new List<string>(){
					"Invalid Credentials"
				},
				Result = false,
			});
		}

		return BadRequest(new AuthResult(){
			Errors = new List<string>(){
				"Invalid payload"
			},
			Result = false
		});
	}

	private async Task<List<Claim>> GetAllValidCliams(IdentityUser user){
		var _option = new IdentityOptions();
		var claims = new List<Claim>(){
				new Claim("id", user.Id),
				new Claim(JwtRegisteredClaimNames.Sub, user.Email),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
		};
		var userClaims = await _userManager.GetClaimsAsync(user);
		claims.AddRange(userClaims);
		var userRoles = await _userManager.GetRolesAsync(user);
		foreach(var userRole in userRoles){
			var role = await _roleManager.FindByNameAsync(userRole);
			if(role != null){
			claims.Add(new Claim(ClaimTypes.Role, userRole));
				var roleClaims = await _roleManager.GetClaimsAsync(role);
				foreach(var roleClaim in roleClaims){
					claims.Add(roleClaim);
				}
			}
		}
		return claims;
	}

	private async Task<string> GenerateJwtToken(IdentityUser user)
	{
		var jwtTokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);
		var claims = await GetAllValidCliams(user);
		var tokenDescriptor = new SecurityTokenDescriptor(){
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.Now.AddHours(1),
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
		};
		var token = jwtTokenHandler.CreateToken(tokenDescriptor);
		var jwtToken = jwtTokenHandler.WriteToken(token);
		return jwtToken;
	}
}
