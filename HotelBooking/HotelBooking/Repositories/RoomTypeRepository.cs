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
    Task CreateAsync(RoomType roomType);
    Task UpdateAsync(RoomType roomType);
    Task DeleteAsync(RoomType roomType);
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

    public async Task CreateAsync(RoomType roomType)
    {
        await _context.RoomTypes.AddAsync(roomType);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RoomType roomType)
    {
        _context.RoomTypes.Update(roomType);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(RoomType roomType)
    {
        _context.RoomTypes.Remove(roomType);
        await _context.SaveChangesAsync();
    }
}