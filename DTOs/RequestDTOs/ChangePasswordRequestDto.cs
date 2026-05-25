namespace ChessTournamentCalendarBackend.API.DTOs.RequestDTOs;

public record ChangePasswordRequestDto(
    string CurrentPassword,
    string NewPassword
);