using HotelBooking.DTO.RequestDto;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using HotelBooking.Repositories;

namespace HotelBooking.Services;

public interface IReservationRoomTypeService
{
    Task AddRoomTypesAsync(int reservationId, List<ReservationRoomTypeDto> roomTypes);
    Task UpdateRoomTypeAsync(int reservationId, List<ReservationRoomTypeDto> newRoomTypes);
}

public class ReservationRoomTypeService : IReservationRoomTypeService
{
    private readonly IReservationRoomTypeRepository _repository;

    public ReservationRoomTypeService(IReservationRoomTypeRepository repository)
    {
        _repository = repository;
    }
    
    public async Task AddRoomTypesAsync(int reservationId, List<ReservationRoomTypeDto> roomTypes)
    {
        if (roomTypes == null && !roomTypes.Any())
            throw new NotFoundException("Room types are empty or not found");
        
        var reservationRoomTypes = roomTypes.Select(roomType => new ReservationRoomType
        {
            ReservationId = reservationId,
            RoomTypeId = roomType.RoomTypeId,
            ReservedRoomCount = roomType.ReservedRoomCount
        }).ToList();

        await _repository.AddReservationRoomTypesAsync(reservationRoomTypes);
        await _repository.SaveChangesAsync();
    }

    public async Task UpdateRoomTypeAsync(int reservationId, List<ReservationRoomTypeDto> newRoomTypes)
    {
        var oldRoomTypes = await _repository.GetByReservationIdAsync(reservationId);

        _repository.RemoveRange(oldRoomTypes);

        await AddRoomTypesAsync(reservationId, newRoomTypes);
    }
}