using AutoMapper;
using ChessTournamentCalendarBackend.API.DTOs.RequestDTOs;
using ChessTournamentCalendarBackend.API.DTOs.ResponseDTOs;
using ChessTournamentCalendarBackend.API.Repositories.Interfaces;
using ChessTournamentCalendarBackend.API.Services.Interfaces;

namespace ChessTournamentCalendarBackend.API.Services.Impl;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : _mapper.Map<UserResponseDto>(user);
    }
    public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user == null ? null : _mapper.Map<UserResponseDto>(user);
    }

    public async Task<PagedResult<UserResponseDto>> GetAllUsersAsync(int pageNumber, int pageSize)
    {
        // Enforce basic validation to prevent invalid SQL queries
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Hard limit for maximum page size

        var (users, totalCount) = await _userRepository.GetAllUsersAsync(pageNumber, pageSize);
        
        var userDtos = _mapper.Map<IEnumerable<UserResponseDto>>(users);

        return new PagedResult<UserResponseDto>(userDtos, totalCount, pageNumber, pageSize);
    }

    public async Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateProfileRequestDto request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        _mapper.Map(request, user);
        await _userRepository.UpdateAsync(user);

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<DeleteUserResponseDto> DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return new DeleteUserResponseDto(false);

        await _userRepository.DeleteAsync(user);
        return new DeleteUserResponseDto(true);
    }
}