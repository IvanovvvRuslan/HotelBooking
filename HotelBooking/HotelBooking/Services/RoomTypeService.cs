using HotelBooking.DTO.ResponseDto;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using HotelBooking.Repositories;
using Mapster;
using Microsoft.AspNetCore.JsonPatch;

namespace HotelBooking.Services;

public interface IRoomTypeService
{
    Task<IEnumerable<RoomTypeDto>> GetAllRoomTypesAsync();
    Task<RoomTypeDto> GetRoomTypeByIdAsync(int id);
    Task CreateRoomTypeAsync(RoomTypeDto roomType);
    Task PatchRoomTypeAsync(int id, JsonPatchDocument<RoomTypeDto> patchDoc);
    Task DeleteRoomTypeAsync(int id);
    
}

public class RoomTypeService : IRoomTypeService
{
    private readonly IRoomTypeRepository _roomTypeRepository;

    public RoomTypeService(IRoomTypeRepository roomTypeRepository)
    {
        _roomTypeRepository = roomTypeRepository;
    }

    public async Task<IEnumerable<RoomTypeDto>> GetAllRoomTypesAsync()
    {
        var roomTypes = await _roomTypeRepository.GetAllAsync(); 
        
        var roomTypesDto = roomTypes.Adapt<IEnumerable<RoomTypeDto>>();
        
        return roomTypesDto;
    }

    public async Task<RoomTypeDto> GetRoomTypeByIdAsync(int id)
    {
        var roomtype = await _roomTypeRepository.GetByIdAsync(id);
        
        if (roomtype == null)
            throw new NotFoundException("Room type not found");
        
        var roomTypeDto = roomtype.Adapt<RoomTypeDto>();
        
        return roomTypeDto;
    }

    public async Task CreateRoomTypeAsync(RoomTypeDto roomType)
    {
        var newRoomType = new RoomType
        {
            Name = roomType.Name,
            Price = roomType.Price,
            MaxOccupancy = roomType.MaxOccupancy,
            Description = roomType.Description
        };
        
        await _roomTypeRepository.CreateAsync(newRoomType);
    }

    public async Task PatchRoomTypeAsync(int id, JsonPatchDocument<RoomTypeDto> patchDoc)
    {
        var roomType = await _roomTypeRepository.GetByIdAsync(id);

        if (roomType == null)
            throw new NotFoundException("Room type not found");
        
        var roomTypeDto = roomType.Adapt<RoomTypeDto>();    //Mapster 
        patchDoc.ApplyTo(roomTypeDto);
        
        roomType.Name = roomTypeDto.Name;
        roomType.Price = roomTypeDto.Price;
        roomType.MaxOccupancy = roomTypeDto.MaxOccupancy;
        roomType.Description = roomTypeDto.Description;
        
        await _roomTypeRepository.UpdateAsync(roomType);
    }

    public async Task DeleteRoomTypeAsync(int id)
    {
        var roomType = await _roomTypeRepository.GetByIdAsync(id);

        if (roomType == null)
            throw new NotFoundException("Room type not found");
        
        await _roomTypeRepository.DeleteAsync(roomType);
    }
}