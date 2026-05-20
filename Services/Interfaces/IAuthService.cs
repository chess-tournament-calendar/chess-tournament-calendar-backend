using ChessTournamentCalendarBackend.API.DTOs.RequestDTOs;
using ChessTournamentCalendarBackend.API.DTOs.ResponseDTOs;

namespace ChessTournamentCalendarBackend.API.Services.Interfaces;

public interface IAuthService
{
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto?> RefreshTokenAsync(string accessToken, string refreshToken);
    
    int RefreshTokenExpirationDays { get; }
}