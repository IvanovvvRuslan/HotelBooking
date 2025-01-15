using Microsoft.AspNetCore.Identity;
using MyWebApp.Data;
using MyWebApp.DTO;
using MyWebApp.Exceptions;
using MyWebApp.Models;

namespace MyWebApp.Services;

public interface IUserService
{
    Task<AuthDto> RegisterUserAsync(CreateUserDto createUserDto);
   
}

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;

    public UserService(UserManager<User> userManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }
    
    public async Task<AuthDto> RegisterUserAsync(CreateUserDto createUserDto)
    {
        var newUser = new User
        {
            Account = new Account
            {
                CreatedAt = DateTime.UtcNow,
                Description = createUserDto.AccountDescription
            },
            UserName = createUserDto.UserName,
            Email = createUserDto.Email
        };
        
        var result = await _userManager.CreateAsync(newUser, createUserDto.Password);

        if (!result.Succeeded)
        {
            string errors = string.Join(",", 
                result.Errors.Select(x => x.Description));
            
            throw new SignUpFailedException(errors);
        }

        return new AuthDto
        {
            AccessToken = _jwtService.GenerateJwt(newUser.Id, newUser.UserName)
        };

    }
}

