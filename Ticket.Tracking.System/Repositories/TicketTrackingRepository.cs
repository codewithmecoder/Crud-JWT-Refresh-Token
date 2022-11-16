using Microsoft.EntityFrameworkCore;
using Ticket.Tracking.System.Data;
using Ticket.Tracking.System.Models;
using Ticket.Tracking.System.RepositoryInterfaces;

namespace Ticket.Tracking.System.Repositories;

public class TicketTrackingRepository : BaseEfRepository<TicketTracking>, ITicketTrackingRepository
{
    public TicketTrackingRepository(AppDbContext dbContext) : base(dbContext) { }

    public async Task CreateTicketTrackingAsync(TicketTracking ticket)
    {
        await _dbContext.TicketTrackings!.AddAsync(ticket);
        _dbContext.SaveChanges();
    }

    public async Task DeleteTicketTrackingAsync(TicketTracking ticket)
    {
        _dbContext.TicketTrackings!.Remove(ticket);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<TicketTracking?> GetTicketTrackingByIdAsync(int id)
    {
        return await _dbContext.TicketTrackings!.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<TicketTracking>> GetTicketTrackingsAsync()
    {
        var tickets = await _dbContext.TicketTrackings!.ToListAsync();
        return tickets;
    }

    public async Task UpdateTicketTrackingAsync(TicketTracking ticket)
    {
        _dbContext.TicketTrackings!.Update(ticket);
        await _dbContext.SaveChangesAsync();
    }
}
