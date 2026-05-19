using System.Security.Claims;
using ChessTournamentCalendarBackend.API.Entities;

namespace ChessTournamentCalendarBackend.API.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    int AccessTokenExpirationMinutes { get; }
    int RefreshTokenExpirationDays { get; }
}