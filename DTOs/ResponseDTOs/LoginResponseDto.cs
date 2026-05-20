namespace ChessTournamentCalendarBackend.API.DTOs.ResponseDTOs;

public record LoginResponseDto (string AccessToken, DateTime AccessTokenExpiresAt);