using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ticket.Tracking.System.Data;
using System.Security.Claims;

namespace Ticket.Tracking.System.Controllers;



[ApiController]
[Route("api/[controller]")]
public class ClaimsSetupController : ControllerBase
{
    
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<SetupController> _logger;

    public ClaimsSetupController(
        AppDbContext context,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<SetupController> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }      

    [HttpGet]
    public async Task<IActionResult> GetAllClaims(string email){
        var user = await _userManager.FindByEmailAsync(email);
        if(user == null){
            _logger.LogInformation($"The email {email} does not exist!");
            return BadRequest(new {Error = "User does not exist!"});
        }
        var userClaims = await _userManager.GetClaimsAsync(user);
        return Ok(userClaims);
    }

    [HttpPost("AddClaimToUser")]
    public async Task<IActionResult> AddClaimToUser(string email, string claimName, string claimValue){
        var user = await _userManager.FindByEmailAsync(email);
        if(user == null){
            _logger.LogInformation($"The email {email} does not exist!");
            return BadRequest(new {Error = "User does not exist!"});
        }
        var userClaim = new Claim(claimName, claimValue);
        var result = await _userManager.AddClaimAsync(user, userClaim);
        if(result.Succeeded){
            return Ok(new {
                Result = $"User {user.UserName} has a claim {claimName} added to them!"
            });
        }
        return BadRequest(new {
            Error = $"Unable to add claim {claimName} to the user {user.UserName}"
        });
    }

    [HttpPost("RemoveClaimFromUser")]
    public async Task<IActionResult> RemoveClaimFromUser(string email, string claimName, string claimValue){
        var user = await _userManager.FindByEmailAsync(email);
        if(user == null){
            _logger.LogInformation($"The email {email} does not exist!");
            return BadRequest(new {Error = "User does not exist!"});
        }
        var userClaims = await _userManager.GetClaimsAsync(user);
        var claim = userClaims.FirstOrDefault(i=> i.Type == claimName && i.Value == claimValue);
        if(claim == null){
            _logger.LogInformation($"Claim {claimValue} does not exist in {claimName}");
            return BadRequest(new {Error = $"Claim {claimValue} does not exist in {claimName}"});
        }
        await _userManager.RemoveClaimAsync(user, claim);
        return Ok(new {Result = "Remove claim successfully"});
    }
}