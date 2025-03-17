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
    private const string AccessToken = "access_token";
    private const int UserId = 1;
    private const string Email = "email@email.com";
    private const string Password = "Password";
    private const string FirstName = "Name";
    private const string LastName = "LastName";
    private string UserName = string.Concat(FirstName, LastName);
    
    [Fact]
    public async Task CreateAsync_ShouldRunSuccessfully()
    {
        //Arrange
        var isAdmin = false;
        var userManager = GetUserManager<User>();
        var jwtService = Substitute.For<IJwtService>();
        var userRepository = Substitute.For<IUserRepository>();
        var adminRepository = Substitute.For<IAdminRepository>();
        var clientRepository = Substitute.For<IClientRepository>();
        
        jwtService.GenerateToken(Arg.Any<int>(), Arg.Any<string>())
            .Returns(AccessToken);
        userManager.CreateAsync(Arg.Any<User>(), Arg.Any<string>()).Returns(IdentityResult.Success);
        userManager.FindByEmailAsync(Arg.Any<string>()).Returns((User)null);
        adminRepository.CreateAsync(Arg.Any<Admin>()).Returns(Task.CompletedTask);
        clientRepository.CreateAsync(Arg.Any<Client>()).Returns(Task.CompletedTask);
        userManager.AddToRoleAsync(Arg.Any<User>(), Arg.Any<string>()).Returns(IdentityResult.Success);
        //using var dbContext = GetDbContext(nameof(CreateAsync_ShouldRunSuccessfully));
        
        var userService = new UserService(userManager, jwtService, null, userRepository, adminRepository, clientRepository);

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
        var userManager = GetUserManager<User>();
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
        var userManager = GetUserManager<User>();
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
        var userManager = GetUserManager<User>();
        var jwtService = Substitute.For<IJwtService>();
        var userRepository = Substitute.For<IUserRepository>();
        var dbContext = GetDbContext(nameof(LoginAsync_ShouldRunSuccessfully));
        
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        
        userManager.CheckPasswordAsync(user, Password).Returns(true);
        jwtService.GenerateToken(user.Id, Password).Returns(AccessToken);
        
        var userService = new UserService(userManager, jwtService, dbContext, userRepository, null, null);
        
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
        var userManager = GetUserManager<User>();
        var dbContext = GetDbContext(nameof(LoginAsync_ShouldThrowIfUserNotFound));
        var userService = new UserService(userManager, null, dbContext, null, null, null);
        
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
        var userManager = GetUserManager<User>();
        var dbContext = GetDbContext(nameof(LoginAsync_ShouldThrowIfPasswordIsInvalid));

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
        userRepository.SaveChangesAsync().Returns(Task.CompletedTask);
        
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

    private ApplicationDbContext GetDbContext(string name)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(name)
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        
        return new ApplicationDbContext(options);
    }
    
    public static UserManager<TUser> GetUserManager<TUser>()
        where TUser : class
    {
        var store = Substitute.For<IUserStore<TUser>>();
        var passwordHasher = Substitute.For<IPasswordHasher<TUser>>();
        IList<IUserValidator<TUser>> userValidators = new List<IUserValidator<TUser>>
        {
            new UserValidator<TUser>()
        };
        IList<IPasswordValidator<TUser>> passwordValidators = new List<IPasswordValidator<TUser>>
        {
            new PasswordValidator<TUser>()
        };
        userValidators.Add(new UserValidator<TUser>());
        passwordValidators.Add(new PasswordValidator<TUser>());
        var userManager = Substitute.For<UserManager<TUser>>(store, 
            null, passwordHasher, userValidators, passwordValidators, null, null, null, null);
        return userManager;
    }
}