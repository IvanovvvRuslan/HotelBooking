using HotelBooking.Data;
using HotelBooking.DTO;
using HotelBooking.DTO.ResponseDto;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using HotelBooking.Repositories;
using HotelBooking.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;

namespace HotelBooking.Tests;

public class UserServiceTests
{
    private static readonly string AccessToken = "access_token";
    private static readonly int UserId = 1;
    private static readonly string Email = "email@email.com";
    private static readonly string Password = "Password";
    private static readonly string FirstName = "Name";
    private static readonly string LastName = "LastName";
    private static readonly string UserName = string.Concat(FirstName, LastName);
    
    [Fact]
    public async Task CreateAsync_ShouldRunSuccessfully()
    {
        //Arrange
        var isAdmin = false;
        var userManager = Utils.GetUserManager<User>();
        var jwtService = Substitute.For<IJwtService>();
        var clientRepository = Substitute.For<IClientRepository>();
        
        jwtService.GenerateToken(Arg.Any<int>(), Arg.Any<string>())
            .Returns(AccessToken);
        userManager.CreateAsync(Arg.Any<User>(), Arg.Any<string>()).Returns(IdentityResult.Success);
       
        var userService = new UserService(userManager, jwtService, null, null, null, clientRepository);

        //Act
        var response = await userService.CreateAsync(new SignUpDto
        {
            Email = Email,
            Password = Password,
            FirstName = FirstName,
            LastName = LastName
        }, isAdmin);
        //Assert
        Assert.Equal(response.AccessToken, AccessToken);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowIfUserFailedToCreate()
    {
        //Arrange
        var isAdmin = false;
        var userManager = Utils.GetUserManager<User>();
        userManager.CreateAsync(Arg.Is<User>(u => u.UserName == UserName), 
            Arg.Is<string>(s => s == Password))
                .Returns(IdentityResult.Failed(new IdentityError ()));
        
        var userService = new UserService(userManager, null, null, null, null, null);
        
        //Act & Assert
        await Assert.ThrowsAsync<SignUpFailedException>(async () => await userService.CreateAsync(new SignUpDto
        {
            Email = Email,
            Password = Password,
            FirstName = FirstName,
            LastName = LastName
        }, isAdmin));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowIfUserAlreadyExists()
    {
        //Arrange
        var isAdmin = false;
        var userManager = Utils.GetUserManager<User>();
        userManager.FindByEmailAsync(Arg.Any<string>()).Returns(new User());
        
        var userService = new UserService(userManager, null, null, null, null, null);
        
        //Act & Assert
        await Assert.ThrowsAsync<SignUpFailedException>(async () => await userService.CreateAsync(new SignUpDto
        {
            Email = Email,
            Password = Password,
            FirstName = FirstName,
            LastName = LastName
        }, isAdmin));
    }

    [Fact]
    public async Task LoginAsync_ShouldRunSuccessfully()
    {
        //Arrange
        var user = new User
        {
            Id = UserId, 
            Email = Email,
            FirstName = FirstName,
            LastName = LastName,
            UserName = UserName
        };
        var userManager = Utils.GetUserManager<User>();
        var jwtService = Substitute.For<IJwtService>();
        var dbContext = Utils.GetDbContext(nameof(LoginAsync_ShouldRunSuccessfully));
        
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        
        userManager.CheckPasswordAsync(user, Password).Returns(true);
        jwtService.GenerateToken(user.Id, Password).Returns(AccessToken);
        
        var userService = new UserService(userManager, jwtService, dbContext, null, null, null);
        
        //Act
        var response = await userService.LoginAsync(new SignInDto
        {
            Email = Email,
            Password = Password
        });
        
        //Assert
        Assert.Equal(AccessToken, response.AccessToken);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowIfUserNotFound()
    {
        //Arrange
        var dbContext = Utils.GetDbContext(nameof(LoginAsync_ShouldThrowIfUserNotFound));
        var userService = new UserService(null, null, dbContext, null, null, null);
        
        //Act & Assert
        await Assert.ThrowsAsync<SignInFailedException>(async () => await userService.LoginAsync(new SignInDto
        {
            Email = Email,
            Password = Password
        }));
    }
    
    [Fact]
    public async Task LoginAsync_ShouldThrowIfPasswordIsInvalid()
    {
        //Arrange
        var user = new User()
        {
            Id = UserId,
            Email = Email,
            FirstName = FirstName,
            LastName = LastName,
            UserName = UserName
        };
        var userManager = Utils.GetUserManager<User>();
        var dbContext = Utils.GetDbContext(nameof(LoginAsync_ShouldThrowIfPasswordIsInvalid));

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        
        userManager.CheckPasswordAsync(user, Password).Returns(false);
        var userService = new UserService(userManager, null, dbContext, null, null, null);
        
        //Act & Assert
        await Assert.ThrowsAsync<SignInFailedException>(async () => await userService.LoginAsync(new SignInDto
        {
            Email = Email,
            Password = Password
        }));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser()
    {
        //Arrange
        var user = new User
        {
            Id = UserId,
            FirstName = "OldName",
            LastName = "OldLast",
            Email = "old@email.com"
        };
        var userRepository = Substitute.For<IUserRepository>();
        userRepository.GetByIdTrackedAsync(UserId).Returns(user);
        
        var userService = new UserService(null, null, null, userRepository, null, null);

        var updateDto = new UserDto
        {
            FirstName = FirstName,
            LastName = LastName,
            Email = Email
        };
        
        //Act
        await userService.UpdateAsync(UserId, updateDto);
        
        //Assert
        Assert.Equal(FirstName, user.FirstName);
        Assert.Equal(LastName, user.LastName);
        Assert.Equal(Email, user.Email);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowIfUserNotFound()
    {
        // Arrange
        var userRepository = Substitute.For<IUserRepository>();
        userRepository.GetByIdTrackedAsync(UserId).Returns((User)null);
        var userService = new UserService(null, null, null, userRepository, null, null);
        var updateDto = new UserDto();
        
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await userService.UpdateAsync(UserId, updateDto));
    }
}