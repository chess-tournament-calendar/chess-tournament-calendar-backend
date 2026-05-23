namespace ChessTournamentCalendarBackend.API.DTOs.ResponseDTOs;

public record PagedResult<T>(
    IEnumerable<T> Items, 
    int TotalCount, 
    int PageNumber, 
    int PageSize
);