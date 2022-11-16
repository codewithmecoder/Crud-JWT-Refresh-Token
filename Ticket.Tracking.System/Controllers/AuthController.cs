using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ticket.Tracking.System.Data;
using Ticket.Tracking.System.Models;
using Ticket.Tracking.System.Models.DTOS;

namespace Ticket.Tracking.System.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
          UserManager<IdentityUser> userManager,
          IConfiguration configuration,
          RoleManager<IdentityRole> roleManager,
          ILogger<AuthController> logger)
    {
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
            if (user_exist != null)
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
            if (isCreated.Succeeded)
            {
                //
                var roleExist = await _roleManager.RoleExistsAsync(user.RoleName ?? "AppUser");
                if (!roleExist)
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(user.RoleName ?? "AppUser"));
                    if (!roleResult.Succeeded)
                    {
                        _logger.LogInformation($"The role {user.RoleName ?? "AppUser"} has not been added successfully");
                        return BadRequest(new { Error = $"The role {user.RoleName ?? "AppUser"} has not been added successfully" });
                    }
                    _logger.LogInformation($"The role {user.RoleName ?? "AppUser"} has been added successfully");

                }
                await _userManager.AddToRoleAsync(new_user, user.RoleName ?? "AppUser");
                var token = await GenerateJwtToken(new_user);
                return Ok(new AuthResult()
                {
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
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDto user)
    {
        if (ModelState.IsValid)
        {
            var existing_user = await _userManager.FindByEmailAsync(user.Email);
            if (existing_user == null)
            {
                return BadRequest(new AuthResult()
                {
                    Errors = new List<string>(){
                        "Invalid Credentials"
                    },
                    Result = false,
                });
            }

            var is_correct = await _userManager.CheckPasswordAsync(existing_user, user.Password);
            if (is_correct)
            {
                var jwtToken = await GenerateJwtToken(existing_user);
                return Ok(new AuthResult()
                {
                    Token = jwtToken,
                    Result = true,
                });
            }
            return BadRequest(new AuthResult()
            {
                Errors = new List<string>(){
                    "Invalid Credentials"
                },
                Result = false,
            });
        }

        return BadRequest(new AuthResult()
        {
            Errors = new List<string>(){
                "Invalid payload"
            },
            Result = false
        });
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponseDto>> GetMe()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        string id = claimsIdentity?.FindFirst("id")?.Value ?? "";
        var user = await _userManager.FindByIdAsync(id);
        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);
        return Ok(new UserResponseDto()
        {
            Id = id,
            Email = user.Email,
            NormalizedEmail = user.NormalizedEmail,
            NormalizedUserName = user.NormalizedUserName,
            UserCliams = claims.Select(i => new UserCliamDto() { ClaimType = i.Type, ClaimValue = i.Value }).ToList(),
            UserRoles = roles.ToList(),
            UserName = user.UserName,
        });
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("AllUsers")]
    public async Task<ActionResult<List<UserResponseDto>>> GetAllUsers()
    {
        List<UserResponseDto> userList = new();

        var users = await _userManager.Users.ToListAsync();
        foreach(var user in users)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            userList.Add(new UserResponseDto()
            {
                Email = user.Email,
                Id = user.Id,
                NormalizedEmail = user.NormalizedEmail,
                NormalizedUserName = user.NormalizedUserName,
                UserCliams = claims.Select(i => new UserCliamDto() { ClaimType = i.Type, ClaimValue = i.Value }).ToList(),
                UserName = user.UserName,
                UserRoles = roles.ToList(),
            });
        }
        return Ok(userList);
    }

    private async Task<List<Claim>> GetAllValidCliams(IdentityUser user)
    {
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
        foreach (var userRole in userRoles)
        {
            var role = await _roleManager.FindByNameAsync(userRole);
            if (role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                foreach (var roleClaim in roleClaims)
                {
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
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };
        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);
        return jwtToken;
    }
}
