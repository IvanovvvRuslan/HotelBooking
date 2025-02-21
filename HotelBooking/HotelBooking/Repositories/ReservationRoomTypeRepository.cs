using HotelBooking.Data;
using HotelBooking.Models;

namespace HotelBooking.Repositories;

public interface IReservationRoomTypeRepository
{
    Task AddReservationRoomTypesAsync(IEnumerable<ReservationRoomType> reservationRoomTypes);
    Task SaveChangesAsync();
}

public class ReservationRoomTypeRepository : IReservationRoomTypeRepository
{
    private readonly ApplicationDbContext _context;

    public ReservationRoomTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddReservationRoomTypesAsync(IEnumerable<ReservationRoomType> reservationRoomTypes)
    {
        await _context.ReservationRoomTypes.AddRangeAsync(reservationRoomTypes);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}