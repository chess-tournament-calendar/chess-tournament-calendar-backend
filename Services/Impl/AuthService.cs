using System.Security.Claims;
using AutoMapper;
using ChessTournamentCalendarBackend.API.DTOs.RequestDTOs;
using ChessTournamentCalendarBackend.API.DTOs.ResponseDTOs;
using ChessTournamentCalendarBackend.API.Entities;
using ChessTournamentCalendarBackend.API.Repositories.Interfaces;
using ChessTournamentCalendarBackend.API.Services.Interfaces;

namespace ChessTournamentCalendarBackend.API.Services.Impl;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUserRepository userRepository,
        IMapper mapper,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _jwtService = jwtService;
    }

    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            // User already exists, return response with IsSuccess = false
            return new RegisterResponseDto(IsSuccess: false);
        }

        var user = _mapper.Map<User>(request);
        user.Password = request.Password; 

        await _userRepository.AddAsync(user);

        // Registration successful, return response with IsSuccess = true
        return new RegisterResponseDto(IsSuccess: true);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null || user.Password != request.Password)
            return null;

        return await GenerateTokensAsync(user);
    }
    
    public async Task<AuthResponseDto?> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(accessToken);
        if (principal == null) return null;

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return null;

        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        if (user == null) return null;

        // DB'deki token ile Cookie'den gelen token eşleşiyor mu kontrolü
        if (user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            return null;

        return await GenerateTokensAsync(user);
    }

    private async Task<AuthResponseDto> GenerateTokensAsync(User user)
    {
        var accessToken = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtService.AccessTokenExpirationMinutes);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtService.RefreshTokenExpirationDays);

        await _userRepository.UpdateAsync(user);

        return new AuthResponseDto(accessToken, refreshToken, expiresAt);
    }
    public int RefreshTokenExpirationDays => _jwtService.RefreshTokenExpirationDays;
}