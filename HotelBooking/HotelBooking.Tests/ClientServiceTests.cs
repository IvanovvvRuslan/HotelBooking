using System.Security.Claims;
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
    private const int UserId = 1;
    private const string PhoneNumber = "1234567890";
    private const string Email = "client@example.com";
    
    [Fact]
    public async Task GetCurrentAsync_ShouldReturnSuccess()
    {
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
        var userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId.ToString());
        
        var clientRepository = Substitute.For<IClientRepository>();
        clientRepository.GetByUserIdAsync(UserId).Returns((Client)null);
        
        var clientService = new ClientService(clientRepository, null, userContext, null);
        
        //Act && Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await clientService.GetCurrentAsync(new ClaimsPrincipal()));
    }

    [Fact]
    public async Task GetCurrentAsync_ShouldThrowIfUserNotFound()
    {
        var userManager = Utils.GetUserManager<User>(); 
        
        var userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId.ToString());
        
        var clientRepository = Substitute.For<IClientRepository>();
        clientRepository.GetByUserIdAsync(UserId).Returns(new Client());
        
        var clientUser= userManager.FindByIdAsync("a").Returns((User)null);
        
        var clientService = new ClientService(clientRepository, userManager, userContext, null);
        
        //Act && Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => clientService.GetCurrentAsync(new ClaimsPrincipal()));
    }
}