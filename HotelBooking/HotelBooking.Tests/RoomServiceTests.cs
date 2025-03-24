using HotelBooking.DTO.RequestDto;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using HotelBooking.Repositories;
using HotelBooking.Services;
using NSubstitute;

namespace HotelBooking.Tests;

public class RoomServiceTests
{
    private const int RoomTypeId = 1;
    private const string RoomNumber = "101";
    private const string Description = "RoomService Description";
    
    [Fact]
    public async Task CreateAsync_ShouldCreateRoomSuccessfully()
    {
        //Arrange
        var roomRepository = Substitute.For<IRoomRepository>();
        var roomService = new RoomService(roomRepository);
        
        var roomDto = new RoomDto
        {
            RoomTypeId = RoomTypeId,
            RoomNumber = RoomNumber,
            Description = Description,
        };
        //Act
       await roomService.CreateAsync(roomDto);
       
       //Assert
       await roomRepository.Received(1).CreateAsync(Arg.Is<Room>(r =>
            r.RoomTypeId == roomDto.RoomTypeId &&
            r.RoomNumber == roomDto.RoomNumber && 
            r.Description == roomDto.Description));
    }

    [Fact]
    public async Task UpdateAsync_ShouldRunSuccessfully()
    {
        //Arrange
        var roomRepository = Substitute.For<IRoomRepository>();
        var existingRoom = new Room
        {
            RoomTypeId = RoomTypeId,
            RoomNumber = RoomNumber,
            Description = Description,
        };
        roomRepository.GetByIdTrackedAsync(1).Returns(existingRoom);
        
        var roomService = new RoomService(roomRepository);

        var updateDto = new RoomDto
        {
            RoomTypeId = 2,
            RoomNumber = "202",
            Description = "New Description",
        };
        
        //Act
        await roomService.UpdateAsync(1, updateDto);
        
        //Assert
        Assert.Equal(updateDto.RoomTypeId, existingRoom.RoomTypeId);
        Assert.Equal(updateDto.RoomNumber, existingRoom.RoomNumber);
        Assert.Equal(updateDto.Description, existingRoom.Description);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowIfRoomNotFound()
    {
        //Arrange
        var roomRepository = Substitute.For<IRoomRepository>();
        roomRepository.GetByIdTrackedAsync(1).Returns((Room)null);
        var roomService = new RoomService(roomRepository);
        
        //Act
        await Assert.ThrowsAsync<NotFoundException>(async () => await roomService.UpdateAsync(1, new RoomDto()));
    }
}