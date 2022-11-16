namespace Ticket.Tracking.System.Models.DTOS;

public class UserResponseDto
{
    public string? Id { get; set; }
    public string? UserName { get; set; }
    public string? NormalizedUserName { get; set; }
    public string? Email { get; set; }
    public string? NormalizedEmail{ get; set; }
    public List<UserCliamDto> UserCliams { get; set; } = new List<UserCliamDto>();
    public List<string> UserRoles { get; set; } = new List<string>();
}

public class UserCliamDto
{
    public string? ClaimType { get; set; }
    public string? ClaimValue { get; set; }
}