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
    Task CreateRoomTypeAsync(RoomTypeDto roomTypeDto);
    Task PatchRoomTypeAsync(int id, RoomTypeDto roomTypeDto);
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

    public async Task CreateRoomTypeAsync(RoomTypeDto roomTypeDto)
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

    public async Task PatchRoomTypeAsync(int id, RoomTypeDto roomTypeDto)
    {
        var roomType = await _roomTypeRepository.GetByIdTrackedAsync(id);

        if (roomType == null)
            throw new NotFoundException("Room type not found");
        
        if (UpdateIfChanged(roomType, roomTypeDto))
        {
            await _roomTypeRepository.SaveChangesAsync();    
        }
        else
        {
            throw new NoChangesException("No changes to update");
        }
    }

    public async Task DeleteRoomTypeAsync(int id)
    {
        var roomType = await _roomTypeRepository.GetByIdAsync(id);

        if (roomType == null)
            throw new NotFoundException("Room type not found");
        
        await _roomTypeRepository.DeleteAsync(roomType);
        await _roomTypeRepository.SaveChangesAsync();
    }

    private bool UpdateIfChanged(RoomType roomTypeOld, RoomTypeDto roomTypeNew)
    {
        bool isUpdated = false;

        if (roomTypeOld.Name != roomTypeNew.Name)
        {
            roomTypeOld.Name = roomTypeNew.Name;
            isUpdated = true;
        }

        if (roomTypeOld.Price != roomTypeNew.Price)
        {
            roomTypeOld.Price = roomTypeNew.Price;
            isUpdated = true;
        }

        if (roomTypeOld.MaxOccupancy != roomTypeNew.MaxOccupancy)
        {
            roomTypeOld.MaxOccupancy = roomTypeNew.MaxOccupancy;
            isUpdated = true;
        }

        if (roomTypeOld.Description != roomTypeNew.Description)
        {
            roomTypeOld.Description = roomTypeNew.Description;
            isUpdated = true;
        }
        
        return isUpdated;
    }
}