using HotelBooking.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;

namespace HotelBooking.Tests;

public class Utils
{
    public static ApplicationDbContext GetDbContext(string name)
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