using System.ComponentModel.DataAnnotations;

namespace ChessTournamentCalendarBackend.API.DTOs.RequestDTOs;

public record LoginRequestDto(
    [Required][EmailAddress] string Email, 
    [Required] string Password
);