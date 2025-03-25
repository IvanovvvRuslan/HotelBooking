using HotelBooking.DTO.ResponseDto;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using HotelBooking.Repositories;
using HotelBooking.Services;
using NSubstitute;

namespace HotelBooking.Tests;

public class RoomTypeServiceTests
{
    private static readonly string Name = "RoomTypeName";
    private static readonly decimal Price = 10.0m;
    private static readonly byte MaxOccupancy = 5;
    private static readonly string Description = "RoomDescription";

    [Fact]
    public async Task CreateAsync_ShouldCreateRoomType()
    {
        //Arrange
        var repository = Substitute.For<IRoomTypeRepository>();
        var service = new RoomTypeService(repository);

        var roomTypeDto = new RoomTypeDto
        {
            Name = Name,
            Price = Price,
            MaxOccupancy = MaxOccupancy,
            Description = Description
        };

        //Act
        await service.CreateAsync(roomTypeDto);

        //Assert
        await repository.Received(1).CreateAsync(Arg.Is<RoomType>(rt =>
            rt.Name == roomTypeDto.Name &&
            rt.Price == roomTypeDto.Price &&
            rt.MaxOccupancy == roomTypeDto.MaxOccupancy &&
            rt.Description == roomTypeDto.Description
        ));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateRoomType()
    {
        //Arrange
        var repository = Substitute.For<IRoomTypeRepository>();

        var oldRoomType = new RoomType
        {
            Name = Name,
            Price = Price,
            MaxOccupancy = MaxOccupancy,
            Description = Description
        };

        repository.GetByIdTrackedAsync(1).Returns(oldRoomType);

        var newRoomTypeDto = new RoomTypeDto
        {
            Name = "NewName",
            Price = 20.0m,
            MaxOccupancy = 10,
            Description = "NewDescription"
        };
        
        var service = new RoomTypeService(repository);
        
        //Act
        await service.UpdateAsync(1, newRoomTypeDto);
        
        //Assert
        Assert.Equal(newRoomTypeDto.Name, oldRoomType.Name);
        Assert.Equal(newRoomTypeDto.Price, oldRoomType.Price);
        Assert.Equal(newRoomTypeDto.MaxOccupancy, oldRoomType.MaxOccupancy);
        Assert.Equal(newRoomTypeDto.Description, oldRoomType.Description);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowIfRoomTypeNotfound()
    {
        //Arrange
        var repository = Substitute.For<IRoomTypeRepository>();
        repository.GetByIdTrackedAsync(1).Returns((RoomType)null);
        
        var service = new RoomTypeService(repository);

        var roomTypeDto = new RoomTypeDto();
        
        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await service.UpdateAsync(1, roomTypeDto));
    }

    [Fact]
    public async Task GetPriceAsync_ShouldReturnPrice()
    {
        //Arrange
        var repository = Substitute.For<IRoomTypeRepository>();
        var expectedPrice = 100.0m;
        
        repository.GetRoomPriceAsync(1).Returns(expectedPrice);
        
        var service = new RoomTypeService(repository);
        
        //Act
        var actualPrice = await service.GetPriceAsync(1);
        
        //Assert
        Assert.Equal(expectedPrice, actualPrice);
    }
    
    [Fact]
    public async Task GetPriceAsync_ShouldThrowIfPriceNotFound()
    {
        //Arrange
        var repository = Substitute.For<IRoomTypeRepository>();
        decimal expectedPrice = 0;
        
        repository.GetRoomPriceAsync(1).Returns(expectedPrice);
        
        var service = new RoomTypeService(repository);
        
        //Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await service.GetPriceAsync(1));
    }
    
    [Fact]
    public async Task GetMaxOccupancyAsync_ShouldReturnPrice()
    {
        //Arrange
        var repository = Substitute.For<IRoomTypeRepository>();
        
        byte expectedOccupancy = 5;
        repository.GetMaxOccupancyAsync(1).Returns(expectedOccupancy);
        
        var service = new RoomTypeService(repository);
        
        //Act
        var actualOccupancy = await service.GetMaxOccupancyAsync(1);
        
        //Assert
        Assert.Equal(expectedOccupancy, actualOccupancy);
    }
    
    
}