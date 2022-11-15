﻿using System.ComponentModel.DataAnnotations;

namespace Ticket.Tracking.System.Models.DTOS;

public class UserRegisterationRequestDto
{
    [Required]
    public string? Name { get; set; }
    [Required]
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
    [Required]
    public string? RoleName { get; set; } 
}
