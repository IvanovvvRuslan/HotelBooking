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
}