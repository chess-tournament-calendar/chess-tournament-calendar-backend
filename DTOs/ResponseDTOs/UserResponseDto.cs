namespace ChessTournamentCalendarBackend.API.DTOs.ResponseDTOs;

public record UserResponseDto(
    Guid Id, 
    string Name, 
    string Email, 
    string Role, 
    string Title, 
    DateTime CreatedAt
);