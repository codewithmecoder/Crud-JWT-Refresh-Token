using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticket.Tracking.System.Data;
using Ticket.Tracking.System.Models;

namespace Ticket.Tracking.System.Controllers;


[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize]
public class TeamController : ControllerBase
{
    
    //private List<Team> teams = new()
    //{
    //    new Team()
    //    {
    //        Country = "Cambodia",
    //        Description = "Hello sdfn;sa",
    //        Id = 1,
    //        Name = "Bek Sloy",
    //        TeamPrinciple = "GGG"
    //    },
    //    new Team()
    //    {
    //        Country = "Cambodia1",
    //        Description = "Hello sdfn;sa",
    //        Id = 2,
    //        Name = "Bek Sloy",
    //        TeamPrinciple = "GGG"
    //    },
    //    new Team()
    //    {
    //        Country = "Cambodia2",
    //        Description = "Hello sdfn;sa",
    //        Id = 3,
    //        Name = "Bek Sloy",
    //        TeamPrinciple = "GGG"
    //    },
    //    new Team()
    //    {
    //        Country = "Cambodia3",
    //        Description = "Hello sdfn;sa",
    //        Id = 4,
    //        Name = "Bek Sloy",
    //        TeamPrinciple = "GGG"
    //    }
    //};
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
