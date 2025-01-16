using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.DTO;
using MyWebApp.Exceptions;
using MyWebApp.Models;

namespace MyWebApp.Services;

public interface IUserService
{
    Task<AuthModel> RegisterUserAsync(SignUpDto signUpDto);
    Task<AuthModel> LoginUserAsync(SignInDto signInDto);
}

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;
    private readonly ApplicationDbContext _context;

    public UserService(UserManager<User> userManager, IJwtService jwtService, ApplicationDbContext context)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _context = context;
    }
    
    public async Task<AuthModel> RegisterUserAsync(SignUpDto signUpDto)
    {
        var newUser = new User
        {
            Account = new Account
            {
                CreatedAt = DateTime.UtcNow,
                Description = signUpDto.AccountDescription
            },
            UserName = signUpDto.UserName,
            Email = signUpDto.Email
        };

        newUser.Account.UserId = newUser.Id;
        
        var result = await _userManager.CreateAsync(newUser, signUpDto.Password);

        if (!result.Succeeded)
        {
            string errors = string.Join(",", 
                result.Errors.Select(x => x.Description));
            
            throw new SignUpFailedException(errors);
        }

        return new AuthModel
        {
            AccessToken = _jwtService.GenerateJwt(newUser.Id, newUser.UserName)
        };
    }

    public async Task<AuthModel> LoginUserAsync(SignInDto signInDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == signInDto.UserName);
        
        if (user == null)
            throw new SignInFailedException("User not found");
        
        var isPasswordValid = await _userManager.CheckPasswordAsync(user!, signInDto.Password);
        
        if (!isPasswordValid)
            throw new SignInFailedException("Password is not valid");
        
        return new AuthModel
        {
            AccessToken = _jwtService.GenerateJwt(user.Id, user!.UserName)
        };
    }
}

