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
[Authorize] 
public class UserController : BaseController 
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Policy = "RequireAdminRole")] 
    public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var users = await _userService.GetAllUsersAsync(pageNumber, pageSize);

        return Ok(
            ApiResponse<PagedResult<UserResponseDto>>.SuccessResponse(
                users,
                "Users retrieved successfully."
            )
        );
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        
        if (!IsAuthorizedToAccessUser(id))
        {
            return StatusCode(StatusCodes.Status403Forbidden, 
                ApiResponse<object>.ErrorResponse("You are not authorized to view this profile.")
            );
        }
        
        var user = await _userService.GetUserByIdAsync(id);
        
        if (user == null)
        {
            return NotFound(
                ApiResponse<object>.ErrorResponse("User not found.")
            );
        }

        return Ok(
            ApiResponse<UserResponseDto>.SuccessResponse(
                user, 
                "User retrieved successfully."
            )
        );
    }
    
    [HttpGet("by-email")]
    public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
    {
        var user = await _userService.GetUserByEmailAsync(email);

        if (user == null)
        {
            return NotFound(
                ApiResponse<object>.ErrorResponse("User not found.")
            );
        }

        
        if (!IsAuthorizedToAccessUser(user.Id))
        {
            return StatusCode(StatusCodes.Status403Forbidden, 
                ApiResponse<object>.ErrorResponse("You are not authorized to view this profile.")
            );
        }

        return Ok(
            ApiResponse<UserResponseDto>.SuccessResponse(
                user,
                "User retrieved successfully."
            )
        );
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateProfileRequestDto request)
    {
        
        if (!IsAuthorizedToAccessUser(id))
        {
            return StatusCode(StatusCodes.Status403Forbidden, 
                ApiResponse<object>.ErrorResponse("You are not authorized to update this profile.")
            );
        }

        var updatedUser = await _userService.UpdateUserAsync(id, request);
        
        if (updatedUser == null)
        {
            return NotFound(
                ApiResponse<object>.ErrorResponse("User not found.")
            );
        }

        return Ok(
            ApiResponse<UserResponseDto>.SuccessResponse(
                updatedUser, 
                "User updated successfully."
            )
        );
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await _userService.DeleteUserAsync(id);

        if (!result.IsSuccess)
        {
            return NotFound(
                ApiResponse<object>.ErrorResponse("User not found.")
            );
        }

        return Ok(
            ApiResponse<object>.SuccessResponse(
                null,
                "User deleted successfully."
            )
        );
    }
}