using System.Security.Claims;
using HotelBooking.DTO.RequestDto;
using HotelBooking.DTO.ResponseDto;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using HotelBooking.Repositories;
using HotelBooking.Services;
using NSubstitute;

namespace HotelBooking.Tests;

public class ReservationServiceTests
{
    private static readonly int UserId = 1;
    private static readonly int ClientId = 1;
    private static readonly int Id = 1;
    private static readonly DateTime CheckInDate = DateTime.Today;
    private static readonly DateTime CheckOutDate = DateTime.Today.AddDays(1);
    private static readonly string Status = "Reserved";
    private static readonly decimal TotalPrice = 10.0m;
    private static readonly byte GuestCount = 2;
    private static readonly string Description = "Description";
    private static readonly int RoomTypeId = 1;
    private static readonly byte ReservedRoomCount = 1;
    
    [Fact]
    public async Task GetAllWithRoomTypesAsync_ShouldReturnSuccessfully()
    {
        //Arrange
        var repository = Substitute.For<IReservationRepository>();

        var reservations = new List<Reservation>
        {
            new Reservation { ClientId = 1, Status = "First", GuestCount = 2 },
            new Reservation { ClientId = 2, Status = "Second", GuestCount = 3 }
        };
        
        repository.GetAllWithRoomTypesAsync().Returns(reservations);

        var service = new ReservationService(repository, null, 
            null, null, null, 
            null, null);
        
        //Act
        var result = await service.GetAllWithRoomTypesAsync();
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(reservations.Count(), result.Count());
        
        var resultList = result.ToList();
        Assert.Equal(reservations[0].ClientId, resultList[0].ClientId);
        Assert.Equal(reservations[1].ClientId, resultList[1].ClientId);
        Assert.Equal(reservations[0].Status, resultList[0].Status);
        Assert.Equal(reservations[1].Status, resultList[1].Status);
        Assert.Equal(reservations[0].GuestCount, resultList[0].GuestCount);
        Assert.Equal(reservations[1].GuestCount, resultList[1].GuestCount);
    }

    [Fact]
    public async Task GetByIdWithRoomTypesAsync_ShouldReturnReservation()
    {
        //Arrange
        var repository = Substitute.For<IReservationRepository>();
        
        var reservation = new Reservation
        {
            ClientId = ClientId,
            CheckInDate = CheckInDate,
            CheckOutDate = CheckOutDate,
            Status = Status, 
            GuestCount = GuestCount,
        };
        
        repository.GetByIdWithRoomTypesAsync(1).Returns(reservation);

        var service = new ReservationService(repository, null, 
            null, null, null, 
            null, null);
        
        //Act
        var result = await service.GetByIdWithRoomTypesAsync(1);
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(ClientId, result.ClientId);
        Assert.Equal(CheckInDate, result.CheckInDate);
        Assert.Equal(CheckOutDate, result.CheckOutDate);
        Assert.Equal(Status, result.Status);
        Assert.Equal(GuestCount, result.GuestCount);
    }

    [Fact]
    public async Task GetByIdWithRoomTypesAsync_ShouldThrowIfReservationNotFound()
    {
        //Arrange
        var repository = Substitute.For<IReservationRepository>();
        repository.GetByIdWithRoomTypesAsync(1).Returns((Reservation)null);
        
        var service = new ReservationService(repository, null, 
            null, null, null, 
            null, null);
        
        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await service.GetByIdWithRoomTypesAsync(1));
    }

    [Fact]
    public async Task GetAllCurrentAsync_ShouldReturnSuccessfully()
    {
        //Arrange
        var userContext = Substitute.For<IUserContext>();
        var clientRepository = Substitute.For<IClientRepository>();
        var repository = Substitute.For<IReservationRepository>();

        var userId = UserId;
        var client = new Client { Id = ClientId };
        
        userContext.UserId.Returns(userId.ToString());
        clientRepository.GetByUserIdAsync(client.Id).Returns(client);
        
        var reservations = new List<Reservation>
        {
            new Reservation { Status = "First", GuestCount = 2 },
            new Reservation { Status = "Second", GuestCount = 3 }
        };
            
        repository.GetAllByClientAsync(client.Id).Returns(reservations);
        
        var service = new ReservationService(repository, clientRepository, 
            null, null, null, 
            userContext, null);
        
        //Act
        var result = await service.GetAllCurrentAsync(new ClaimsPrincipal());
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(reservations.Count, result.Count());
        
        var resultList = result.ToList();
        Assert.Equal(reservations[0].Status, resultList[0].Status);
        Assert.Equal(reservations[0].GuestCount, resultList[0].GuestCount);
        Assert.Equal(reservations[1].Status, resultList[1].Status);
        Assert.Equal(reservations[1].GuestCount, resultList[1].GuestCount);
    }

    [Fact]
    public async Task GetAllCurrentAsync_ShouldThrowIfClientNotFound()
    {
        //Arrange
        var userContext = Substitute.For<IUserContext>();
        var clientRepository = Substitute.For<IClientRepository>();
        var repository = Substitute.For<IReservationRepository>();
        
        userContext.UserId.Returns(UserId.ToString());

        clientRepository.GetByUserIdAsync(UserId).Returns((Client)null);
        
        var service = new ReservationService(repository, clientRepository, 
            null, null, null, 
            userContext, null);
        
        //Act
        await Assert.ThrowsAsync<NotFoundException>(async () => await service.GetAllCurrentAsync(new ClaimsPrincipal()));
    }

    [Fact]
    public async Task GetCurrentByIdAsync_ShouldReturnSuccessfully()
    {
        //Arrange
        var userContext = Substitute.For<IUserContext>();
        var clientRepository = Substitute.For<IClientRepository>();
        var repository = Substitute.For<IReservationRepository>();
        
        userContext.UserId.Returns(UserId.ToString());
        
        var client = new Client { Id = ClientId };
        
        clientRepository.GetByUserIdAsync(client.Id).Returns(client);

        var reservation = new Reservation
        {
            Id = Id, 
            CheckInDate = CheckInDate,
            CheckOutDate = CheckOutDate,
            ClientId = ClientId, 
            Status = Status, 
            GuestCount = GuestCount
        };

        repository.GetByIdClientAsync(reservation.Id, reservation.ClientId).Returns(reservation);
        
        var service = new ReservationService(repository, clientRepository, 
            null, null, null, 
            userContext, null);
        
        //Act
        var result = await service.GetCurrentByIdAsync(Id, new ClaimsPrincipal());
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(CheckInDate, result.CheckInDate);
        Assert.Equal(CheckOutDate, result.CheckOutDate);
        Assert.Equal(Status, result.Status);
        Assert.Equal(GuestCount, result.GuestCount);
    }

    [Fact]
    public async Task GetCurrentByIdAsync_ShouldThrowIfClientNotFound()
    {
        //Arrange
        var userContext = Substitute.For<IUserContext>();
        var clientRepository = Substitute.For<IClientRepository>();
        
        userContext.UserId.Returns(UserId.ToString());
        clientRepository.GetByUserIdAsync(1).Returns((Client)null);
        
        var service = new ReservationService(null, clientRepository, 
            null, null, null, 
            userContext, null);
        
        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await service.GetCurrentByIdAsync(Id, new ClaimsPrincipal()));
    }

    [Fact]
    public async Task GetCurrentByIdAsync_ShouldThrowIfReservationNotFound()
    {
        //Arrange
        var userContext = Substitute.For<IUserContext>();
        var clientRepository = Substitute.For<IClientRepository>();
        var repository = Substitute.For<IReservationRepository>();
        
        userContext.UserId.Returns(UserId.ToString());
        var client = new Client { Id = ClientId };
        
        clientRepository.GetByUserIdAsync(UserId).Returns(client);
        repository.GetByIdClientAsync(Id, client.Id).Returns((Reservation)null);
        
        var service = new ReservationService(repository, clientRepository, 
            null, null, null, 
            userContext, null);
        
        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await service.GetCurrentByIdAsync(Id, new ClaimsPrincipal()));
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateSuccessfully()
    {
        //Arrange
        var newReservation = new ReservationForAdminDto
        {
            ClientId = ClientId,
            CheckInDate = CheckInDate,
            CheckOutDate = CheckOutDate,
            Status = Status,
            TotalPrice = TotalPrice,
            GuestCount = GuestCount,
            Description = Description,
            RoomTypes = new List<ReservationRoomTypeDto>
            {
                new ReservationRoomTypeDto
                {
                    RoomTypeId = RoomTypeId,
                    ReservedRoomCount = ReservedRoomCount
                }
            }
        };
        
        var roomTypeService = Substitute.For<IRoomTypeService>();
        var repository = Substitute.For<IReservationRepository>();
        var dbContext = Utils.GetDbContext(nameof(CreateAsync_ShouldCreateSuccessfully));
        var reservationRoomTypeService = Substitute.For<IReservationRoomTypeService>();
        
        roomTypeService.GetMaxOccupancyAsync(RoomTypeId).Returns(GuestCount);
        roomTypeService.GetPriceAsync(RoomTypeId).Returns(TotalPrice);
        repository.IsRoomTypeAvailable(RoomTypeId, CheckInDate, CheckOutDate).Returns((byte)1);
        
        var service = new ReservationService(repository, null,
            null, reservationRoomTypeService, roomTypeService,
            null, dbContext);
        
        // Act
        await service.CreateAsync(newReservation);
        
        //Assert
        await repository.Received(1).CreateAsync(Arg.Any<Reservation>());
        await repository.Received(1).SaveChangesAsync();
        await reservationRoomTypeService.Received(1)
            .AddAsync(Arg.Any<int>(), Arg.Any<List<ReservationRoomTypeDto>>());
        await reservationRoomTypeService.Received(1).AddAsync(Arg.Any<int>(), newReservation.RoomTypes);
        
        await repository.Received(1).CreateAsync(Arg.Is<Reservation>(r =>
            r.ClientId == newReservation.ClientId &&
            r.CheckInDate == newReservation.CheckInDate &&
            r.CheckOutDate == newReservation.CheckOutDate &&
            r.Status == newReservation.Status &&
            r.TotalPrice == newReservation.TotalPrice &&
            r.GuestCount == newReservation.GuestCount &&
            r.Description == newReservation.Description
        ));
    }
    
    [Fact]
    public async Task CreateCurrentAsync_ShouldCreateSuccessfully()
    {
        //Arrange
        var userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId.ToString());
        
        var clientRepository = Substitute.For<IClientRepository>();
        var client = new Client { Id = ClientId };
        clientRepository.GetByUserIdAsync(1).Returns(client);
        
        var newReservation = new ReservationForClientCreateDto
        {
            
            CheckInDate = CheckInDate,
            CheckOutDate = CheckOutDate,
            GuestCount = GuestCount,
            Description = Description,
            RoomTypes = new List<ReservationRoomTypeDto>
            {
                new ReservationRoomTypeDto
                {
                    RoomTypeId = RoomTypeId,
                    ReservedRoomCount = ReservedRoomCount
                }
            }
        };
        
        var roomTypeService = Substitute.For<IRoomTypeService>();
        var repository = Substitute.For<IReservationRepository>();
        var dbContext = Utils.GetDbContext(nameof(CreateCurrentAsync_ShouldCreateSuccessfully));
        var reservationRoomTypeService = Substitute.For<IReservationRoomTypeService>();
        
        roomTypeService.GetMaxOccupancyAsync(RoomTypeId).Returns(GuestCount);
        roomTypeService.GetPriceAsync(RoomTypeId).Returns(TotalPrice);
        repository.IsRoomTypeAvailable(RoomTypeId, CheckInDate, CheckOutDate).Returns((byte)1);
        
        var service = new ReservationService(repository, clientRepository,
            null, reservationRoomTypeService, roomTypeService,
            userContext, dbContext);
        
        // Act
        await service.CreateCurrentAsync(new ClaimsPrincipal(), newReservation);
        
        //Assert
        await repository.Received(1).CreateAsync(Arg.Any<Reservation>());
        await repository.Received(1).SaveChangesAsync();
        await reservationRoomTypeService.Received(1)
            .AddAsync(Arg.Any<int>(), Arg.Any<List<ReservationRoomTypeDto>>());
        await reservationRoomTypeService.Received(1).AddAsync(Arg.Any<int>(), newReservation.RoomTypes);
        
        await repository.Received(1).CreateAsync(Arg.Is<Reservation>(r =>
            r.CheckInDate == newReservation.CheckInDate &&
            r.CheckOutDate == newReservation.CheckOutDate &&
            r.GuestCount == newReservation.GuestCount &&
            r.Description == newReservation.Description
        ));
    }

    [Fact]
    public async Task CreateCurrentAsync_ShouldThrowIfClientNotFound()
    {
        //Arrange
        var userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId.ToString());
        var clientRepository = Substitute.For<IClientRepository>();
        clientRepository.GetByUserIdAsync(1).Returns((Client)null);
        
        var service = new ReservationService(null, clientRepository,
            null, null, null,
            userContext, null);
        
        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await service.CreateCurrentAsync(new ClaimsPrincipal(), null));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateSuccessfully()
    {
        //Arrange
        var repository = Substitute.For<IReservationRepository>();
        var roomTypeService = Substitute.For<IRoomTypeService>();
        var reservationRoomTypeService = Substitute.For<IReservationRoomTypeService>();
        var dbContext = Utils.GetDbContext(nameof(UpdateAsync_ShouldUpdateSuccessfully));

        var oldReservation = new Reservation
        {
            ClientId = ClientId,
            CheckInDate = CheckInDate,
            CheckOutDate = CheckOutDate,
            Status = Status,
            TotalPrice = TotalPrice,
            GuestCount = GuestCount,
            Description = Description
        };

        repository.GetByIdTrackedAsync(Id).Returns(oldReservation);

        var newReservation = new ReservationForAdminDto
        {
            ClientId = 2,
            CheckInDate = CheckInDate.AddDays(1),
            CheckOutDate = CheckOutDate.AddDays(1),
            Status = "Updated",
            TotalPrice = TotalPrice + 10m,
            GuestCount = (byte)(GuestCount + 1),
            Description = "Updated Description",
            RoomTypes = new List<ReservationRoomTypeDto>
            {
                new ReservationRoomTypeDto
                {
                    RoomTypeId = RoomTypeId,
                    ReservedRoomCount = ReservedRoomCount
                }
            }
        };

        roomTypeService.GetMaxOccupancyAsync(RoomTypeId).Returns(newReservation.GuestCount);

        var service = new ReservationService(repository, null,
            null, reservationRoomTypeService, roomTypeService,
            null, dbContext);
        
        //Act
        await service.UpdateAsync(Id, newReservation);
        
        //Assert
        await repository.Received(1).SaveChangesAsync();
        await reservationRoomTypeService.Received(1)
            .UpdateAsync(Arg.Any<int>(), Arg.Any<List<ReservationRoomTypeDto>>());
        
        Assert.Equal(newReservation.ClientId, oldReservation.ClientId);
        Assert.Equal(newReservation.CheckInDate, oldReservation.CheckInDate);
        Assert.Equal(newReservation.CheckOutDate, oldReservation.CheckOutDate);
        Assert.Equal(newReservation.Status, oldReservation.Status);
        Assert.Equal(newReservation.TotalPrice, oldReservation.TotalPrice);
        Assert.Equal(newReservation.GuestCount, oldReservation.GuestCount);
        Assert.Equal(newReservation.Description, oldReservation.Description);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowIfReservationNotFound()
    {
        //Arrange
        var repository = Substitute.For<IReservationRepository>();
        repository.GetByIdTrackedAsync(Id).Returns((Reservation)null);
        
        var service = new ReservationService(repository, null,
            null, null, null,
            null, null);
        
        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await service.UpdateAsync(Id, null));
    }

    [Fact]
    public async Task UpdateCurrentAsync_ShouldUpdateSuccessfully()
    {
        var userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId.ToString());
        
        var clientRepository = Substitute.For<IClientRepository>();
        var client = new Client { Id = ClientId };
        clientRepository.GetByUserIdAsync(1).Returns(client);
        
        var repository = Substitute.For<IReservationRepository>();
        
        var oldReservation = new Reservation
        {
            ClientId = ClientId,
            CheckInDate = CheckInDate,
            CheckOutDate = CheckOutDate,
            Status = Status,
            TotalPrice = TotalPrice,
            GuestCount = GuestCount,
            Description = Description
        };
        
        repository.GetByIdClientCurrentAsync(Id, client.Id).Returns(oldReservation);

        var newReservation = new ReservationForClientUpdateDto
        {
            GuestCount = (byte)(GuestCount + 1),
            Description = "Updated Description"
        };
        
        var service = new ReservationService(repository, clientRepository,
            null, null, null,
            userContext, null);
        
        //Act
        await service.UpdateCurrentAsync(Id, newReservation, new ClaimsPrincipal());
        
        //Assert
        await repository.Received(1).SaveChangesAsync();
        
        Assert.Equal(newReservation.GuestCount, oldReservation.GuestCount);
        Assert.Equal(newReservation.Description, oldReservation.Description);
    }

    [Fact]
    public async Task UpdateCurrentAsync_ShouldThrowIfClientNotFound()
    {
        //Arrange
        var userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId.ToString());
        var clientRepository = Substitute.For<IClientRepository>();
        clientRepository.GetByUserIdAsync(1).Returns((Client)null);
        
        var service = new ReservationService(null, clientRepository,
            null, null, null,
            userContext, null);
        
        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await service.UpdateCurrentAsync(Id, null, new ClaimsPrincipal()));
    }
    
    [Fact]
    public async Task UpdateCurrentAsync_ShouldThrowIfReservationNotFound()
    {
        //Arrange
        var userContext = Substitute.For<IUserContext>();
        var clientRepository = Substitute.For<IClientRepository>();
        var repository = Substitute.For<IReservationRepository>();
        
       var client = new Client { Id = ClientId };
        
        userContext.UserId.Returns(UserId.ToString());
        clientRepository.GetByUserIdAsync(1).Returns(client);
        
        repository.GetByIdClientCurrentAsync(Id, client.Id).Returns((Reservation)null);
        
        var service = new ReservationService(repository, clientRepository,
            null, null, null,
            userContext, null);
        
        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await service.UpdateCurrentAsync(Id, null, new ClaimsPrincipal()));
    }

    [Fact]
    public async Task DeleteCurrentAsync_ShouldDeleteSuccessfully()
    {
        //Arrange
        var userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId.ToString());
        
        var clientRepository = Substitute.For<IClientRepository>();
        var client = new Client { Id = ClientId };
        clientRepository.GetByUserIdAsync(1).Returns(client);
        
        var repository = Substitute.For<IReservationRepository>();
        var reservation = new Reservation 
        { 
            Id = Id, 
            ClientId = ClientId 
        };
        repository.GetByIdClientCurrentAsync(Id, client.Id).Returns(reservation);
        
        var service = new ReservationService(repository, clientRepository,
            null, null, null,
            userContext, null);
        
        //Act
        await service.DeleteCurrentAsync(Id, new ClaimsPrincipal());
        
        //Assert
        await repository.Received(1).Delete(reservation);
        await repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task DeleteCurrentAsync_ShouldThrowIfClientNotFound()
    {
        //Arrange
        var userContext = Substitute.For<IUserContext>();   
        userContext.UserId.Returns(UserId.ToString());
        var clientRepository = Substitute.For<IClientRepository>();
        clientRepository.GetByUserIdAsync(1).Returns((Client)null);
        
        var service = new ReservationService(null, clientRepository,
            null, null, null,
            userContext, null);
        
        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await service.DeleteCurrentAsync(Id, new ClaimsPrincipal()));
    }

    [Fact]
    public async Task DeleteCurrentAsync_ShouldThrowIfReservationNotFound()
    {
        //Arrange
        var userContext = Substitute.For<IUserContext>();
        var clientRepository = Substitute.For<IClientRepository>();
        var repository = Substitute.For<IReservationRepository>();

        var client = new Client { Id = ClientId };

        userContext.UserId.Returns(UserId.ToString());
        clientRepository.GetByUserIdAsync(1).Returns(client);

        repository.GetByIdClientCurrentAsync(Id, client.Id).Returns((Reservation)null);

        var service = new ReservationService(repository, clientRepository,
            null, null, null,
            userContext, null);

        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await service.DeleteCurrentAsync(Id, new ClaimsPrincipal()));
    }
}