using HotelBooking.Data;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repositories;

public interface IReservationRepository
{
    Task<IEnumerable<Reservation>> GetAllAsync();
    Task<Reservation> GetByIdAsync(int id);
    Task<Reservation> GetByIdTrackedAsync(int id);
    Task<IEnumerable<Reservation>> GetAllByClientAsync(int id);
    Task<Reservation> GetByIdClientAsync(int id, int clientId);
    Task<Reservation> GetByIdClientCurrentAsync(int id, int clientId);
    Task CreateAsync(Reservation reservation);
    Task DeleteAsync(Reservation reservation);
    Task SaveChangesAsync();
}

public class ReservationRepository : IReservationRepository
{
    private readonly ApplicationDbContext _context;

    public ReservationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Reservation>> GetAllAsync()
    {
        var reservations = await _context.Reservations
            .AsNoTracking()
            .ToListAsync();
        
        return reservations;
    }

    public async Task<Reservation> GetByIdAsync(int id)
    {
        var reservation = await _context.Reservations
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        return reservation;
    }

    public async Task<Reservation> GetByIdTrackedAsync(int id)
    {
        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(r => r.Id == id);

        return reservation;
    }

    public async Task<IEnumerable<Reservation>> GetAllByClientAsync(int id)
    {
        var reservations = await _context.Reservations
            .AsNoTracking()
            .Where(r => r.ClientId == id)
            .ToListAsync();
        
        return reservations;
    }

    public async Task<Reservation> GetByIdClientAsync(int id, int clientId)
    {
        var reservation = await _context.Reservations
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id && r.ClientId == clientId);
        
        return reservation;
    }

    public async Task<Reservation> GetByIdClientCurrentAsync(int id, int clientId)
    {
        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(r => r.Id == id && r.ClientId == clientId);
        
        return reservation;
    }

    public async Task CreateAsync(Reservation reservation)
    {
        await _context.Reservations.AddAsync(reservation);
    }

    public Task DeleteAsync(Reservation reservation)
    {
        _context.Reservations.Remove(reservation);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}