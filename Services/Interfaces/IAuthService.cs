using ChessTournamentCalendarBackend.API.DTOs.RequestDTOs;
using ChessTournamentCalendarBackend.API.DTOs.ResponseDTOs;

namespace ChessTournamentCalendarBackend.API.Services.Interfaces;

public interface IAuthService
{
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto?> RefreshTokenAsync(string accessToken, string refreshToken);

    Task<ChangePasswordResponseDto> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request);
    
    int RefreshTokenExpirationDays { get; }
}