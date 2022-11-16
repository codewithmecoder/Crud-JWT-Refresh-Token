using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ticket.Tracking.System.Models;

namespace Ticket.Tracking.System.Data;

public class AppDbContext:IdentityDbContext
{
	public DbSet<Team>? Teams { get; set; }
	public DbSet<TicketTracking>? TicketTrackings { get; set; }
	public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
	{

	}
}
