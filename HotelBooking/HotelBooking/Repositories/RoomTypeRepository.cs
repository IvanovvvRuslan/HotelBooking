﻿using HotelBooking.Data;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repositories;

public interface IRoomTypeRepository : IGenericRepository<RoomType>
{
    Task<decimal> GetRoomPriceAsync(int id);
    Task<byte> GetMaxOccupancyAsync(int id);
}

public class RoomTypeRepository : GenericRepository<RoomType>, IRoomTypeRepository
{
    private readonly ApplicationDbContext _context;
    public RoomTypeRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<decimal> GetRoomPriceAsync(int id)
    {
        var price = await _context.RoomTypes
            .Where(r => r.Id == id)
            .Select(r => r.Price)
            .FirstOrDefaultAsync();
        
        return price;
    }

    public async Task<byte> GetMaxOccupancyAsync(int id)
    {
        var occupancy = await _context.RoomTypes
            .Where(r => r.Id == id)
            .Select(r => r.MaxOccupancy)
            .FirstOrDefaultAsync();
        
        return occupancy;
    }
}