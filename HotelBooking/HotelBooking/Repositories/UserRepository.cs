using HotelBooking.Data;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByIdAsync(int userId);
    Task<User> GetByIdTrackedAsync(int userId);
    Task DeleteAsync(User user);
    Task SaveChangesAsync();
}

public class UserRepository: IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var users = await _context.Users
            .AsNoTracking()
            .ToListAsync();

        return users;
    }

    public async Task<User> GetByIdAsync(int userId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        return user;
    }

    public async Task<User> GetByIdTrackedAsync(int userId)
    {
        var user = _context.Users
            .FirstOrDefault(u => u.Id == userId);
        
        return user;
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    
}