using HotelBooking.Data;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repositories;

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetallAsync();
    Task<Client> GetByIdAsync(int id);
    Task<Client> GetByIdTrackedAsync(int id);
    Task CreateAsync(Client client);
    Task SaveChangesAsync();
    Task DeleteAsync(Client client);
}

public class ClientRepository : IClientRepository
{
    private readonly ApplicationDbContext _context;

   public ClientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Client>> GetallAsync()
    {
        var clients = await _context.Clients
            .AsNoTracking()
            .ToListAsync();
        
        return clients;
    }

    public async Task<Client> GetByIdAsync(int id)
    {
        var client = await _context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
        
        return client;
    }
    
    public async Task<Client> GetByIdTrackedAsync(int id)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == id);
        
        return client;
    }

    public async Task CreateAsync(Client client)
    {
        await _context.Clients.AddAsync(client);
    }

    public async Task DeleteAsync(Client client)
    {
        _context.Clients.Remove(client);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}