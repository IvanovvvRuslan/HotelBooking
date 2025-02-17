using HotelBooking.Data;
using HotelBooking.Models;

namespace HotelBooking.Repositories;

public interface IClientRepository
{
    Task CreateAsync(Client client);
}

public class ClientRepository : IClientRepository
{
    private readonly ApplicationDbContext _context;

    public ClientRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task CreateAsync(Client client)
    {
        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();
    }
}