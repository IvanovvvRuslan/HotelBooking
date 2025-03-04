using System.Data;
using HotelBooking.Data;
using HotelBooking.DTO.RequestDto;
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
    Task CreateReservationWithTransactionAsync(Reservation reservation,
        List<ReservationRoomTypeDto> reservationRoomTypes);
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

    public async Task CreateReservationWithTransactionAsync(Reservation reservation,
        List<ReservationRoomTypeDto> reservationRoomTypes)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            foreach (var roomType in reservationRoomTypes)
            {
                var availableRoomsCount = await IsRoomTypeAvailable(
                    roomType.RoomTypeId,
                    reservation.CheckInDate,
                    reservation.CheckOutDate);
                
                if (availableRoomsCount < roomType.ReservedRoomCount)
                    throw new InvalidOperationException($"{availableRoomsCount} rooms of this type is/are available for the requested dates.");
            }
            
            await CreateAsync(reservation);
            await SaveChangesAsync();
            
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<byte> IsRoomTypeAvailable(int roomTypeId, DateTime checkIn, DateTime checkOut)
    {
        var reservedCount = await _context.ReservationRoomTypes
            .Where(rrt => rrt.RoomTypeId == roomTypeId
            && checkIn.Date < rrt.Reservation.CheckOutDate.Date
            && checkOut.Date > rrt.Reservation.CheckInDate.Date)
            .SumAsync(rrt => rrt.ReservedRoomCount);
        
        var totalRooms = await _context.Rooms
            .CountAsync(r => r.RoomTypeId == roomTypeId);
        
        return (byte)(totalRooms - reservedCount);
    }
}