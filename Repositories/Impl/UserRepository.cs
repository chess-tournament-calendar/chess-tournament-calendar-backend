using ChessTournamentCalendarBackend.API.Data;
using ChessTournamentCalendarBackend.API.Entities;
using ChessTournamentCalendarBackend.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChessTournamentCalendarBackend.API.Repositories.Impl;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<(IEnumerable<User> Users, int TotalCount)> GetAllUsersAsync(int pageNumber, int pageSize)
    {
        // 1. Get the total number of records for pagination math
        var totalCount = await _context.Users.CountAsync();

        // 2. Fetch only the required slice of data
        var users = await _context.Users
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (users, totalCount);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(x => x.Email == email);
    }
}