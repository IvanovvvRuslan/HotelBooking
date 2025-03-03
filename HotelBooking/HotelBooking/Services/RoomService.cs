using HotelBooking.DTO.RequestDto;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using HotelBooking.Repositories;

namespace HotelBooking.Services;

public interface IRoomService : IGenericService<Room, RoomDto>
{
    Task CreateAsync(RoomDto roomDto);
    Task UpdateAsync(int id, RoomDto roomDto);
}

public class RoomService : GenericService<Room, RoomDto>, IRoomService
{
    private readonly IRoomRepository _repository;
    
    public RoomService(IRoomRepository repository) : base(repository)
    {
        _repository = repository;
    }

    public async Task CreateAsync(RoomDto roomDto)
    {
        var newRoom = new Room
        {
            RoomTypeId = roomDto.RoomTypeId,
            RoomNumber = roomDto.RoomNumber,
            Description = roomDto.Description
        };
        
        await _repository.CreateAsync(newRoom);
        await _repository.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, RoomDto roomDto)
    {
        var oldRoom = await _repository.GetByIdTrackedAsync(id);

        if (oldRoom == null)
            throw new NotFoundException("Room not found");
        
        oldRoom.RoomTypeId = roomDto.RoomTypeId;
        oldRoom.RoomNumber = roomDto.RoomNumber;
        oldRoom.Description = roomDto.Description;
        
        await _repository.SaveChangesAsync();
    }
}