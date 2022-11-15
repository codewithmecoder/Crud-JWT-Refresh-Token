using Microsoft.EntityFrameworkCore;
using Ticket.Tracking.System.Models;

namespace Ticket.Tracking.System.Data;

public class AppDbContext:DbContext
{
	public DbSet<Team>? Teams { get; set; }
	public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
	{

	}
}
