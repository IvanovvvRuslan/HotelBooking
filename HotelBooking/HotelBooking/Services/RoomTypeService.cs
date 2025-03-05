using HotelBooking.DTO.ResponseDto;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using HotelBooking.Repositories;
using Mapster;
using Microsoft.AspNetCore.JsonPatch;

namespace HotelBooking.Services;

public interface IRoomTypeService : IGenericService<RoomType, RoomTypeDto>
{
    Task CreateAsync(RoomTypeDto roomTypeDto);
    Task UpdateAsync(int id, RoomTypeDto roomTypeDto);
    Task<decimal> GetPriceAsync(int id);
    Task<byte> GetMaxOccupancyAsync(int id);
}

public class RoomTypeService : GenericService<RoomType, RoomTypeDto>, IRoomTypeService
{
    private readonly IRoomTypeRepository _roomTypeRepository;

    public RoomTypeService(IRoomTypeRepository roomTypeRepository) : base(roomTypeRepository)
    {
        _roomTypeRepository = roomTypeRepository;
    }

    public async Task CreateAsync(RoomTypeDto roomTypeDto)
    {
        var newRoomType = new RoomType
        {
            Name = roomTypeDto.Name,
            Price = roomTypeDto.Price,
            MaxOccupancy = roomTypeDto.MaxOccupancy,
            Description = roomTypeDto.Description
        };
        
        await _roomTypeRepository.CreateAsync(newRoomType);
        await _roomTypeRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, RoomTypeDto roomTypeDto)
    {
        var oldRoomType = await _roomTypeRepository.GetByIdTrackedAsync(id);

        if (oldRoomType == null)
            throw new NotFoundException("Room type not found");
        
        oldRoomType.Name = roomTypeDto.Name;
        oldRoomType.Price = roomTypeDto.Price;
        oldRoomType.MaxOccupancy = roomTypeDto.MaxOccupancy;
        oldRoomType.Description = roomTypeDto.Description;
        
        await _roomTypeRepository.SaveChangesAsync();
    }

    public async Task<decimal> GetPriceAsync(int id)
    {
        var price = await _roomTypeRepository.GetRoomPriceAsync(id);

        if (price == null && price == 0)
            throw new NotFoundException("Price not found");
        
        return price;
    }

    public async Task<byte> GetMaxOccupancyAsync(int id)
    {
        var occupancy = await _roomTypeRepository.GetMaxOccupancyAsync(id);

        if (occupancy == null && occupancy == 0)
            throw new NotFoundException("Max occupancy not found");
        
        return occupancy;
    }
}