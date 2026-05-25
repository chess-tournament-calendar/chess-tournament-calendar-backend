using ChessTournamentCalendarBackend.API.DTOs.RequestDTOs;
using ChessTournamentCalendarBackend.API.DTOs.ResponseDTOs;

namespace ChessTournamentCalendarBackend.API.Services.Interfaces;

public interface IUserService
{
    Task<UserResponseDto?> GetUserByIdAsync(Guid id);
    Task<UserResponseDto?> GetUserByEmailAsync(string email);
    Task<PagedResult<UserResponseDto>> GetAllUsersAsync(int pageNumber, int pageSize);
    Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateProfileRequestDto request);
    Task<DeleteUserResponseDto> DeleteUserAsync(Guid id);
}