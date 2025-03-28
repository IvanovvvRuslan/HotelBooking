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
    public async Task UpdateAsync_ShouldUpdateSuccessfully()
    {
        var reservationId = 1;
        var repository = Substitute.For<IReservationRoomTypeRepository>();
        
        var oldRoomTypes = new List<ReservationRoomType>
        {
            new ReservationRoomType { ReservationId = 1, RoomTypeId = 1, ReservedRoomCount = 1 },
            new ReservationRoomType { ReservationId = 2, RoomTypeId = 2, ReservedRoomCount = 2 }
        };

        var newRoomTypes = new List<ReservationRoomTypeDto>
        {
            new ReservationRoomTypeDto 
            { 
                RoomTypeId = 11, 
                ReservedRoomCount = 11,
            },
            new ReservationRoomTypeDto 
            { 
                RoomTypeId = 12, 
                ReservedRoomCount = 22 
            }
        };
        
        repository.GetByReservationIdAsync(reservationId).Returns(oldRoomTypes);
        
        var service = new ReservationRoomTypeService(repository);
        
        //Act
        await service.UpdateAsync(reservationId, newRoomTypes);
        
        //Assert
        repository.Received(1).GetByReservationIdAsync(reservationId);
        repository.Received(1).RemoveRange(oldRoomTypes);
        repository.Received(1).AddReservationRoomTypesAsync(Arg.Is<List<ReservationRoomType>>(list =>
            list.Count == 2 &&
            list.Any(r => r.ReservationId == reservationId && r.RoomTypeId == 11 && r.ReservedRoomCount == 11) &&
            list.Any(r => r.ReservationId == reservationId && r.RoomTypeId == 12 && r.ReservedRoomCount == 22)));
        repository.Received(1).SaveChangesAsync();
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