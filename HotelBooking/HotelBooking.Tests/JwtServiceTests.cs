using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotelBooking.Models;
using HotelBooking.Options;
using HotelBooking.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using Microsoft.Extensions.Options;

namespace HotelBooking.Tests;

public class JwtServiceTests
{
    [Fact]
    public void GenerateToken_ShouldReturnValidToken()
    {
        // Arrange
        var userManager = Substitute.For<UserManager<User>>(Substitute.For<IUserStore<User>>(), 
            null, null, null, null, null, null, null, null);
        
        var jwtOptions = Microsoft.Extensions.Options.Options.Create(new JwtOptions
        {
            Issuer = "testIssuer",
            Audience = "testAudience",
            Key = "testKey1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"
        });

        var jwtService = new JwtService(jwtOptions, userManager);

        var userId = 1;
        var userName = "testUser";
        var roles = new List<string> { "Admin", "User" };

        userManager.FindByIdAsync(userId.ToString()).Returns(new User());
        userManager.GetRolesAsync(Arg.Any<User>()).Returns(roles);

        // Act
        var token = jwtService.GenerateToken(userId, userName);

        // Assert
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        Assert.Equal(userId.ToString(), jwtToken.Subject);
        Assert.Equal(userName, jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value);
        
        var roleValues = jwtToken.Claims
            .Where(c => c.Type == "role" )
            .Select(c => c.Value)
            .ToList();
        
        Assert.Equal(roleValues, roles);
        
        Assert.Equal("testIssuer", jwtToken.Issuer);
        Assert.Equal("testAudience", jwtToken.Audiences.First());
    }
}