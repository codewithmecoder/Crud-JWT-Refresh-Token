namespace Ticket.Tracking.System.Models;

public class AuthResult
{
    public string? Token { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public bool Result { get; set; }
}
