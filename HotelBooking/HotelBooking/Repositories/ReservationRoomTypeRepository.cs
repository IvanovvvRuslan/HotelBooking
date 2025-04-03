using HotelBooking.Data;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repositories;

public interface IReservationRoomTypeRepository : IGenericRepository<ReservationRoomType>
{
    Task<IEnumerable<ReservationRoomType>> GetByReservationIdAsync(int reservationId);
    Task AddReservationRoomTypesAsync(IEnumerable<ReservationRoomType> reservationRoomTypes);
    Task RemoveRange(IEnumerable<ReservationRoomType> reservationRoomTypes);
}

public class ReservationRoomTypeRepository : GenericRepository<ReservationRoomType>, IReservationRoomTypeRepository
{
    private readonly ApplicationDbContext _context;

    public ReservationRoomTypeRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<ReservationRoomType>> GetByReservationIdAsync(int reservationId)
    {
        var reservationRoomTypes = await _context.ReservationRoomTypes
            .Where(x => x.ReservationId == reservationId)
            .AsNoTracking()
            .ToListAsync();
        
        return reservationRoomTypes;
    }

    public async Task AddReservationRoomTypesAsync(IEnumerable<ReservationRoomType> reservationRoomTypes)
    {
        await _context.ReservationRoomTypes.AddRangeAsync(reservationRoomTypes);
    }

    public Task RemoveRange(IEnumerable<ReservationRoomType> reservationRoomTypes)
    {
        _context.ReservationRoomTypes.RemoveRange(reservationRoomTypes);
        return Task.CompletedTask;
    }
}