using HotelBooking.Data;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repositories;

public interface IReservationRepository : IGenericRepository<Reservation>
{
    Task<IEnumerable<Reservation>> GetAllByClientAsync(int id);
    Task<Reservation> GetByIdClientAsync(int id, int clientId);
    Task<IEnumerable<Reservation>> GetAllWithRoomTypesAsync();
    Task<Reservation> GetByIdWithRoomTypesAsync(int id);
    Task<Reservation> GetByIdClientCurrentAsync(int id, int clientId);
}

public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
{
    private readonly ApplicationDbContext _context;

    public ReservationRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Reservation>> GetAllByClientAsync(int id)
    {
        var reservations = await _context.Reservations
            .Include(r => r.RoomTypes)
            .AsNoTracking()
            .Where(r => r.ClientId == id)
            .ToListAsync();
        
        return reservations;
    }

    public async Task<Reservation> GetByIdClientAsync(int id, int clientId)
    {
        var reservation = await _context.Reservations
            .Include(r => r.RoomTypes)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id && r.ClientId == clientId);
        
        return reservation;
    }

    public async Task<IEnumerable<Reservation>> GetAllWithRoomTypesAsync()
    {
        var reservations = await _context.Reservations
            .Include(r => r.RoomTypes)
            .AsNoTracking()
            .ToListAsync();
        
        return reservations;
    }

    public Task<Reservation> GetByIdWithRoomTypesAsync(int id)
    {
        var reservation = _context.Reservations
            .AsNoTracking()
            .Include(r => r.RoomTypes)
            .FirstOrDefaultAsync(r => r.Id == id);
            
        return reservation;
    }

    public async Task<Reservation> GetByIdClientCurrentAsync(int id, int clientId)
    {
        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(r => r.Id == id && r.ClientId == clientId);
        
        return reservation;
    }
}