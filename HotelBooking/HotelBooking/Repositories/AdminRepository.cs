using HotelBooking.Data;
using HotelBooking.Models;

namespace HotelBooking.Repositories;

public interface IAdminRepository
{
    Task CreateAsync(Admin admin);
}

public class AdminRepository : IAdminRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AdminRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAsync(Admin admin)
    {
        await _dbContext.Admins.AddAsync(admin);
        await _dbContext.SaveChangesAsync();
    }
}