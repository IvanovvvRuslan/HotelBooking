using HotelBooking.Data;
using HotelBooking.DTO.ResponseDto;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repositories;

public interface IRoomTypeRepository
{
    Task<IEnumerable<RoomType>> GetAllAsync();
    Task<RoomType> GetByIdAsync(int id);
    Task<RoomType> GetByIdTrackedAsync(int id);
    Task CreateAsync(RoomType roomType);
    Task DeleteAsync(RoomType roomType);
    Task SaveChangesAsync();
}

public class RoomTypeRepository : IRoomTypeRepository
{
    private readonly ApplicationDbContext _context;
    private IRoomTypeRepository _roomTypeRepositoryImplementation;

    public RoomTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<RoomType>> GetAllAsync()
    {
        var roomTypes = await _context.RoomTypes
            .AsNoTracking()
            .ToListAsync();
        
        return roomTypes;
    }

    public async Task<RoomType> GetByIdAsync(int id)
    {
        var roomType = await _context.RoomTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        return roomType;
    }

    public async Task<RoomType> GetByIdTrackedAsync(int id)
    {
        var roomType = await _context.RoomTypes
           .FirstOrDefaultAsync(r => r.Id == id);

        return roomType;
    }

    public async Task CreateAsync(RoomType roomType)
    {
        await _context.RoomTypes.AddAsync(roomType);
    }

    public async Task DeleteAsync(RoomType roomType)
    {
        _context.RoomTypes.Remove(roomType);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}