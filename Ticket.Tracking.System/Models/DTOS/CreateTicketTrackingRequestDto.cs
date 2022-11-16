namespace Ticket.Tracking.System.Models.DTOS;

public class CreateTicketTrackingRequestDto
{
    public string Summary { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
