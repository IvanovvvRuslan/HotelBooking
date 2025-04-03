using HotelBooking.Data;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repositories;

public interface IClientRepository : IGenericRepository<Client>
{
    Task<Client> GetByUserIdAsync(int userId);
    Task<Client> GetByUserIdTrackedAsync(int userId);
}

public class ClientRepository : GenericRepository<Client>, IClientRepository
{
    private readonly ApplicationDbContext _context;

   public ClientRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

  public async Task<Client> GetByUserIdAsync(int userId)
    {
        var client = await _context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == userId);
        
        return client;
    }

    public async Task<Client> GetByUserIdTrackedAsync(int userId)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.UserId == userId);
        
        return client;
    }
}