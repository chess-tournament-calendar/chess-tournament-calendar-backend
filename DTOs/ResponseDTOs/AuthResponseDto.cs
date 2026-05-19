namespace ChessTournamentCalendarBackend.API.DTOs.ResponseDTOs;

public record AuthResponseDto(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt);