using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticket.Tracking.System.Data;
using Ticket.Tracking.System.Models;

namespace Ticket.Tracking.System.Controllers;


[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, AppUser")]
public class TeamController : ControllerBase
{
    private readonly AppDbContext _context;
    public TeamController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var teams = await _context.Teams!.ToListAsync();
        return Ok(teams);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var team = await _context.Teams!.FirstOrDefaultAsync(x => x.Id == id);
        if (team == null) return BadRequest("Invalid id");
        return Ok(team);
    }

    [HttpPost()]
    [Authorize(Policy = "DepartmentPolicy")]
    public async Task<IActionResult> Post([FromBody] Team team)
    {
        await _context.Teams!.AddAsync(team);
        _context.SaveChanges();
        return CreatedAtAction("Get", team.Id, team);
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Patch(int id, string country)
    {
        var team = await _context.Teams!.FirstOrDefaultAsync(x => x.Id == id);
        if (team == null) return BadRequest("Invalid id");
        team.Country = country;
        _context.Teams!.Update(team);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var team = await _context.Teams!.FirstOrDefaultAsync(x => x.Id == id);
        if (team == null) return BadRequest("Invalid id");
        _context.Teams!.Remove(team);
        _context.SaveChanges();
        return NoContent();
    }
}
