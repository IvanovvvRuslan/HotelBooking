using System.Security.Claims;
using HotelBooking.DTO;
using HotelBooking.DTO.ResponseDto;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using HotelBooking.Repositories;
using HotelBooking.Services;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace HotelBooking.Tests;

public class ClientServiceTests
{
    private static readonly int UserId = 1;
    private static readonly int ClientId = 1;
    private static readonly string PhoneNumber = "1234567890";
    private static readonly string Gender = "Male";
    private static readonly string Country = "Ukraine";
    
    [Fact]
    public async Task GetCurrentAsync_ShouldReturnSuccess()
    {
        //Arrange
        var userManager = Utils.GetUserManager<User>();
        var client = new Client { UserId = UserId };
        var user = new User {Id = UserId, PhoneNumber = PhoneNumber};
        
        var userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId.ToString());
        
        var clientRepository = Substitute.For<IClientRepository>();
        clientRepository.GetByUserIdAsync(UserId).Returns(client);
        
        userManager.FindByIdAsync(UserId.ToString()).Returns(user);
        
        var clientService = new ClientService(clientRepository, userManager, userContext, null);
       
        //Act
        var result = clientService.GetCurrentAsync(new ClaimsPrincipal());
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(user.PhoneNumber, result.Result.PhoneNumber);
    }

    [Fact]
    public async Task GetCurrentAsync_ShouldThrowIfClientNotFound()
    {
        //Arrange
        var userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId.ToString());
        
        var clientRepository = Substitute.For<IClientRepository>();
        clientRepository.GetByUserIdAsync(UserId).Returns((Client)null);
        
        var clientService = new ClientService(clientRepository, null, userContext, null);
        
        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await clientService.GetCurrentAsync(new ClaimsPrincipal()));
    }

    [Fact]
    public async Task GetCurrentAsync_ShouldThrowIfUserNotFound()
    {
        //Arrange
        var userManager = Utils.GetUserManager<User>(); 
        
       var userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId.ToString());
        
        var clientRepository = Substitute.For<IClientRepository>();
        clientRepository.GetByUserIdAsync(UserId).Returns(new Client());
        
       userManager.FindByIdAsync(UserId.ToString()).Returns((User)null);
        
        var clientService = new ClientService(clientRepository, userManager, userContext, null);
        
        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await clientService.GetCurrentAsync(new ClaimsPrincipal()));
    }

    [Fact]
    public async Task CreateAsync_ShouldRunSuccessfully()
    {
        //Arrange
        var isAdmin = false;
        
        var userService = Substitute.For<IUserService>();
        var clientService = new ClientService(null, null, null, userService);

        var signupDto = new SignUpDto();

        //Act
        await clientService.CreateAsync(signupDto);
        
        //Assert
        await userService.Received(1).CreateAsync(signupDto, isAdmin);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateClient()
    {
        //Arrange
        var clientRepository = Substitute.For<IClientRepository>();
        var userManager = Utils.GetUserManager<User>();
        
        var oldClient = new Client {Gender = "oldGender", Country = "oldCountry"};
        clientRepository.GetByIdTrackedAsync(ClientId).Returns(oldClient);
        
        var user = new User {Id = UserId, PhoneNumber = "00000"};
        userManager.FindByIdAsync(user.Id.ToString()).Returns(user);
        
        var clientService = new ClientService(clientRepository, userManager, null, null);

        var newClient = new ClientForAdminDto
        {
            UserId = UserId,
            Gender = Gender, 
            Country = Country, 
            PhoneNumber = PhoneNumber
        };

        //Act
        await clientService.UpdateAsync(ClientId, newClient);
        
        //Assert
        Assert.Equal(newClient.Gender, oldClient.Gender);
        Assert.Equal(newClient.Country, oldClient.Country);
        Assert.Equal(newClient.PhoneNumber, user.PhoneNumber);
        await clientRepository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowIfClientNotFound()
    {
        //Arrange
        var clientRepository = Substitute.For<IClientRepository>();
        clientRepository.GetByIdTrackedAsync(ClientId).Returns((Client)null);

        var clientDto = new ClientForAdminDto();
        
        var clientService = new ClientService(clientRepository, null, null, null);
        
        //Act &
        await Assert.ThrowsAsync<NotFoundException>(async () => await clientService.UpdateAsync(ClientId, clientDto));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowIfUserNotFound()
    {
        //Arrange
        var clientRepository = Substitute.For<IClientRepository>();
        clientRepository.GetByIdTrackedAsync(ClientId).Returns(new Client());
        
        var userManager = Utils.GetUserManager<User>();
        userManager.FindByIdAsync(UserId.ToString()).Returns((User)null);
        
        var clientDto = new ClientForAdminDto();
        
        var clientService = new ClientService(clientRepository, userManager, null, null);
        
        //Act
        await Assert.ThrowsAsync<NotFoundException>(async () => await clientService.UpdateAsync(ClientId, clientDto));
    }

    [Fact]
    public async Task UpdateCurrentAsync_ShouldUpdateClient()
    {
        var userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId.ToString());
        
        var clientRepository = Substitute.For<IClientRepository>();
        var oldClient = new Client {Gender = "oldGender", Country = "oldCountry"};
        clientRepository.GetByUserIdTrackedAsync(UserId).Returns(oldClient);
        
        var userManager = Utils.GetUserManager<User>();
        var user = new User {Id = UserId, PhoneNumber = "00000"};
        userManager.FindByIdAsync(user.Id.ToString()).Returns(user);
        
        var clientService = new ClientService(clientRepository, userManager, userContext, null);

        var newClient = new ClientForUserDto
        {
            Gender = Gender,
            Country = Country,
            PhoneNumber = PhoneNumber
        };
        
        //Act
        await clientService.UpdateCurrentAsync(new ClaimsPrincipal(), newClient);
        
        //Assert
        Assert.Equal(newClient.Gender, oldClient.Gender);
        Assert.Equal(newClient.Country, oldClient.Country);
        Assert.Equal(newClient.PhoneNumber, user.PhoneNumber);
        await clientRepository.Received(1).SaveChangesAsync();
    }
    
    [Fact]
    public async Task UpdateCurrentAsync_ShouldThrowIfClientNotFound()
    {
        //Arrange
        var userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId.ToString());
        
        var clientRepository = Substitute.For<IClientRepository>();
        clientRepository.GetByIdTrackedAsync(ClientId).Returns((Client)null);

        var clientDto = new ClientForUserDto();
        
        var clientService = new ClientService(clientRepository, null, userContext, null);
        
        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await clientService.UpdateCurrentAsync(new ClaimsPrincipal(), clientDto));
    }

    [Fact]
    public async Task UpdateCurrentAsync_ShouldThrowIfUserNotFound()
    {
        //Arrange
        var userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId.ToString());
        
        var clientRepository = Substitute.For<IClientRepository>();
        clientRepository.GetByIdTrackedAsync(ClientId).Returns(new Client());
        
        var userManager = Utils.GetUserManager<User>();
        userManager.FindByIdAsync(UserId.ToString()).Returns((User)null);
        
        var clientService = new ClientService(clientRepository, userManager, userContext, null);
        var clientDto = new ClientForUserDto();
        
        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await clientService.UpdateCurrentAsync(new ClaimsPrincipal(), clientDto));
    }

    [Fact]
    public async Task DeleteWithUserAsync_ShouldDeleteClient()
    {
        //Arrange
        var clientRepository = Substitute.For<IClientRepository>();
        var userManager = Utils.GetUserManager<User>();
        
        var client = new Client { Id = ClientId, UserId = UserId };
        var user = new User {Id = UserId};
        
        clientRepository.GetByIdAsync(ClientId).Returns(client);
        userManager.FindByIdAsync(UserId.ToString()).Returns(user);
        userManager.DeleteAsync(user).Returns(IdentityResult.Success);
        clientRepository.GetByIdTrackedAsync(ClientId).Returns(client);
        
        var clientService = new ClientService(clientRepository, userManager, null, null);
        
        //Act
        await clientService.DeleteWithUserAsync(ClientId);
        
        //Assert
        await userManager.Received(1).DeleteAsync(user);
        clientRepository.Received(1).Delete(client);
        await clientRepository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task DeleteWithUserAsync_ShouldThrowIfClientNotFound()
    {
        //Arrange
        var clientRepository = Substitute.For<IClientRepository>();
        clientRepository.GetByIdAsync(ClientId).Returns((Client)null);
        
        var clientService = new ClientService(clientRepository, null, null, null);
        
        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await clientService.DeleteWithUserAsync(ClientId));
    }
    
    [Fact]
    public async Task DeleteCurrentWithUserAsync_ShouldDeleteClient()
    {
        //Arrange
        var userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId.ToString());
        
        var clientRepository = Substitute.For<IClientRepository>();
        var userManager = Utils.GetUserManager<User>();
        
        var client = new Client { Id = ClientId, UserId = UserId };
        var user = new User {Id = UserId};
        
        clientRepository.GetByUserIdAsync(ClientId).Returns(client);
        userManager.FindByIdAsync(UserId.ToString()).Returns(user);
        userManager.DeleteAsync(user).Returns(IdentityResult.Success);
        clientRepository.GetByUserIdAsync(ClientId).Returns(client);
        
        var clientService = new ClientService(clientRepository, userManager, userContext, null);
        
        //Act
        await clientService.DeleteCurrentWithUserAsync(new ClaimsPrincipal());
        
        //Assert
        await userManager.Received(1).DeleteAsync(user);
        clientRepository.Received(1).Delete(client);
        await clientRepository.Received(1).SaveChangesAsync();
    }
    
    [Fact]
    public async Task DeleteCurrentWithUserAsync_ShouldThrowIfClientNotFound()
    {
        //Arrange
        var userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId.ToString());
        
        var clientRepository = Substitute.For<IClientRepository>();
        clientRepository.GetByIdAsync(ClientId).Returns((Client)null);
        
        var clientService = new ClientService(clientRepository, null, userContext, null);
        
        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await clientService.DeleteWithUserAsync(ClientId));
    }
}