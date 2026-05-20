using ChessTournamentCalendarBackend.API.Entities;

namespace ChessTournamentCalendarBackend.API.Repositories.Interfaces;

public interface IUserRepository
{
    // READ
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetAllAsync();

    // CREATE
    Task AddAsync(User user);

    // UPDATE
    Task UpdateAsync(User user);

    // DELETE
    Task DeleteAsync(User user);

    // CHECKS
    Task<bool> ExistsByEmailAsync(string email);
}