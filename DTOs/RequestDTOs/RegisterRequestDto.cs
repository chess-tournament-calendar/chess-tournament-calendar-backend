using System.ComponentModel.DataAnnotations;
using ChessTournamentCalendarBackend.API.Enums;

namespace ChessTournamentCalendarBackend.API.DTOs.RequestDTOs;

public record RegisterRequestDto(
    [Required] string Name ,
    [Required][EmailAddress] string Email, 
    [Required][MinLength(6)] string Password, 
    ChessTitle Title = ChessTitle.None
);