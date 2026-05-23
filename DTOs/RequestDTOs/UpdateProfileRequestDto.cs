using ChessTournamentCalendarBackend.API.Enums;

namespace ChessTournamentCalendarBackend.API.DTOs.RequestDTOs;

public record UpdateProfileRequestDto(
    string Name, 
    ChessTitle Title
);