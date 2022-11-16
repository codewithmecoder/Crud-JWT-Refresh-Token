using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticket.Tracking.System.Data;

namespace Ticket.Tracking.System.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
public class RoleSetupController : ControllerBase
{
  private readonly UserManager<IdentityUser> _userManager;
  private readonly RoleManager<IdentityRole> _roleManager;
  private readonly ILogger<RoleSetupController> _logger;

  public RoleSetupController(
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ILogger<RoleSetupController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoles(){
        var roles = await _roleManager.Roles.ToListAsync();
        return Ok(roles);
    }

    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers(){
        var users = await _userManager.Users.ToListAsync();
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(string name){
        var roleExist = await _roleManager.RoleExistsAsync(name);
        if(!roleExist){
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(name));
            if(!roleResult.Succeeded){
                _logger.LogInformation($"The role {name} has not been added successfully");
                return BadRequest(new {Error = $"The role {name} has not been added successfully"});
            }
            _logger.LogInformation($"The role {name} has been added successfully");
            return Ok(new {
                Result = $"The role {name} has been added successfully"
            });
        }
        return BadRequest(new {Error = "Role already exist!"});
    }

    [HttpPost("AddUserToRole")]
    public async Task<IActionResult> AddUserToRole(string email, string roleName){
        var user = await _userManager.FindByEmailAsync(email);
        if(user == null){
            _logger.LogInformation($"The email {email} does not exist!");
            return BadRequest(new {Error = "User does not exist!"});
        }
        var role = await _roleManager.FindByNameAsync(roleName);
        if(role == null){
            _logger.LogInformation($"The role {roleName} does not exist!");
            return BadRequest(new {Error = "Role does not exist!"});
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        if(result.Succeeded){
            return Ok(new {
                Result = "Success, user has been added to role"
            });
        }
        return BadRequest(new {
            Error = "Fail to add user to the role"
        });
    }

    [HttpGet("GetUserRole")]
    public async Task<IActionResult> GetUserRole(string email){
        var user = await _userManager.FindByEmailAsync(email);
        if(user == null){
            _logger.LogInformation($"The email {email} does not exist!");
            return BadRequest(new {Error = "User does not exist!"});
        }
        var roles = await _userManager.GetRolesAsync(user);
        return Ok(roles);
    }

    [HttpPost("RemoveUserFromRole")]
    public async Task<IActionResult> RemoveUserFromRole(string email, string roleName){
        var user = await _userManager.FindByEmailAsync(email);
        if(user == null){
            _logger.LogInformation($"The email {email} does not exist!");
            return BadRequest(new {Error = "User does not exist!"});
        }
        var role = await _roleManager.FindByNameAsync(roleName);
        if(role == null){
            _logger.LogInformation($"The role {roleName} does not exist!");
            return BadRequest(new {Error = "Role does not exist!"});
        }

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        if(result.Succeeded){
            return Ok(new{Result = $"User has been removed from the role {roleName} successfully!"});
        }
        return BadRequest(new {Error = $"Unable to remove user from the role {roleName}!"});
    }
}