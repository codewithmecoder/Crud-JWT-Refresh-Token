namespace Ticket.Tracking.System.Models.DTOS;

public class UpdateTicketTrackingRequestDto
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsSovled { get; set; }
    public DateTime CreatedAt { get; set; }
}
