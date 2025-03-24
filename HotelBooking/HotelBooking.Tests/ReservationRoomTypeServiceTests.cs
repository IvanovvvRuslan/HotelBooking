using HotelBooking.DTO.RequestDto;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using HotelBooking.Repositories;
using HotelBooking.Services;
using NSubstitute;

namespace HotelBooking.Tests;

public class ReservationRoomTypeServiceTests
{
    [Fact]
    public async Task AddAsync_ShouldAddSuccessfully()
    {
        //Arrange
        var repository = Substitute.For<IReservationRoomTypeRepository>();
        var service = new ReservationRoomTypeService(repository);

        var reservationId = 1;

        var roomTypes = new List<ReservationRoomTypeDto>
        {
            new ReservationRoomTypeDto { RoomTypeId = 1, ReservedRoomCount = 1 },
            new ReservationRoomTypeDto { RoomTypeId = 2, ReservedRoomCount = 2 }
        };
        
        //Act
        await service.AddAsync(reservationId, roomTypes);
        
        //Assert
        await repository.Received(1).AddReservationRoomTypesAsync(Arg.Is<List<ReservationRoomType>>(
            list => list.Count == 2 &&
                    list.Any(r => r.RoomTypeId == 1 && r.ReservedRoomCount == 1) &&
                    list.Any(r => r.RoomTypeId == 2 && r.ReservedRoomCount == 2)));
        
        await repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowIfRoomTypeNotFound()
    {
        //Arrange
        var repository = Substitute.For<IReservationRoomTypeRepository>();
        var service = new ReservationRoomTypeService(repository);

        int reservationId = 1;
        
        //Act
        await Assert.ThrowsAsync<NotFoundException>(async () => await service.UpdateAsync(reservationId, new List<ReservationRoomTypeDto>()));
    }
}