using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Ticket.Tracking.System.Models;
using Ticket.Tracking.System.Models.DTOS;
using Ticket.Tracking.System.RepositoryInterfaces;

namespace Ticket.Tracking.System.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TicketController : ControllerBase
{
    private readonly ITicketTrackingRepository _context;
    public TicketController(ITicketTrackingRepository context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<TicketTracking>>> Get()
    {
        var teams = await _context.GetTicketTrackingsAsync();
        return Ok(teams);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketTracking?>> Get(int id)
    {
        var ticket = await _context.GetTicketTrackingByIdAsync(id);
        if (ticket == null) return BadRequest("Invalid id");
        return Ok(ticket);
    }

    [HttpPost()]
    [Authorize(Roles = "Admin, QA")]
    public async Task<IActionResult> Post([FromBody] CreateTicketTrackingRequestDto ticketDto)
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        string id = claimsIdentity?.FindFirst("id")?.Value ?? "";
        TicketTracking ticket = new()
        {
            CreatedAt = DateTime.UtcNow,
            Description = ticketDto.Description,
            Id = 0,
            IsSovled = false,
            Summary = ticketDto.Summary,
            UpdateAt = DateTime.UtcNow,
            UserId = Guid.Parse(id),
        };

        await _context.CreateTicketTrackingAsync(ticket);

        return CreatedAtAction("Get", ticket.Id, ticket);
    }

    [HttpPut()]
    [Authorize(Roles = "Admin, QA")]
    public async Task<IActionResult> Put([FromBody] UpdateTicketTrackingRequestDto ticketDto)
    {
        TicketTracking ticket = new()
        {
            Description = ticketDto.Description,
            Id = ticketDto.Id,
            IsSovled = false,
            Summary = ticketDto.Summary,
            UpdateAt = DateTime.UtcNow,
            UserId = Guid.Parse(ticketDto.UserId!),
            CreatedAt = ticketDto.CreatedAt,

        };
        await _context.UpdateTicketTrackingAsync(ticket);
        return NoContent();
    }

    [HttpPut("/MarkAsSolved/{id:int}/{isSovled:bool}")]
    [Authorize(Roles = "Admin, RD")]
    public async Task<IActionResult> MarkAsSolved(int id, bool isSovled)
    {
        var ticket = await _context.GetTicketTrackingByIdAsync(id);
        if (ticket == null) return BadRequest("Invalid id");
        ticket.IsSovled = isSovled;
        ticket.UpdateAt = DateTime.UtcNow;
        await _context.UpdateTicketTrackingAsync(ticket);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin, QA")]
    public async Task<IActionResult> Delete(int id)
    {
        var team = await _context.GetTicketTrackingByIdAsync(id);
        if (team == null) return BadRequest("Invalid id");
        await _context.DeleteTicketTrackingAsync(team);
        return NoContent();
    }
}
