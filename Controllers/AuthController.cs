using Asp.Versioning;
using ChessTournamentCalendarBackend.API.DTOs;
using ChessTournamentCalendarBackend.API.DTOs.RequestDTOs;
using ChessTournamentCalendarBackend.API.DTOs.ResponseDTOs;
using ChessTournamentCalendarBackend.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChessTournamentCalendarBackend.API.Controllers;

[ApiVersion("1.0")]
[Route("api/{version:apiVersion}/[controller]")]
[ApiController]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);

        // Check the boolean inside our custom response DTO
        if (!result.IsSuccess)
        {
            return BadRequest(
                ApiResponse<object>.ErrorResponse("A user with this email already exists.")
            );
        }

        // Return the RegisterResponseDto inside our standard success wrapper
        return Ok(
            ApiResponse<RegisterResponseDto>.SuccessResponse(
                result,
                "Registration completed successfully. Please proceed to the login page."
            )
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request); 
    
        if (result == null)
        {
            return Unauthorized(
                ApiResponse<object>.ErrorResponse("Invalid credentials.")
            );
        }

        // 1. Set the Refresh Token in a secure HttpOnly cookie (XSS Protection)
        SetRefreshTokenCookie(result.RefreshToken);

        // 2. Return ONLY the AccessToken and Expiry in the JSON body using LoginResponseDto
        var responseData = new LoginResponseDto(
            result.AccessToken, 
            result.AccessTokenExpiresAt
        );

        return Ok(
            ApiResponse<LoginResponseDto>.SuccessResponse(
                responseData,
                "Login successful."
            )
        );
    }
    
    [Authorize] // 👈 Sadece token'ı olan (giriş yapmış) kullanıcılar buraya istek atabilir
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized(
                ApiResponse<object>.ErrorResponse("User identification failed.")
            );
        }

        var userId = Guid.Parse(userIdClaim);

        var result = await _authService.ChangePasswordAsync(userId, request);

        if (!result.IsSuccess)
        {
            return BadRequest(
                ApiResponse<object>.ErrorResponse("Invalid current password.")
            );
        }

        return Ok(
            ApiResponse<object>.SuccessResponse(
                null,
                "Password changed successfully. Please log in again with your new password."
            )
        );
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        // 1. Extract the Refresh Token from the incoming secure cookies
        var refreshTokenFromCookie = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshTokenFromCookie))
        {
            return Unauthorized(
                ApiResponse<object>.ErrorResponse("Refresh token is missing from cookies.")
            );
        }

        // 2. Validate both tokens via the service
        var result = await _authService.RefreshTokenAsync(request.AccessToken, refreshTokenFromCookie);
        
        if (result == null)
        {
            return Unauthorized(
                ApiResponse<object>.ErrorResponse("Invalid or expired token.")
            );
        }

        // 3. Set the NEW Refresh Token in the cookie (Refresh Token Rotation)
        SetRefreshTokenCookie(result.RefreshToken);

        // 4. Return the NEW Access Token to the client using LoginResponseDto
        var responseData = new LoginResponseDto(
            result.AccessToken, 
            result.AccessTokenExpiresAt
        );

        return Ok(
            ApiResponse<LoginResponseDto>.SuccessResponse(
                responseData,
                "Token refreshed successfully."
            )
        );
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, 
            Secure = true,   
            SameSite = SameSiteMode.None, 
            Expires = DateTime.UtcNow.AddDays(_authService.RefreshTokenExpirationDays) 
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}