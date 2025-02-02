using HotelBooking.Data;
using HotelBooking.DTO;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Services;

public interface IUserService
{
    Task<AuthDto> CreateUserAsync(SignUpDto signUpDto);
    Task<AuthDto> LoginUserAsync(SignInDto signInDto);
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
    
    public async Task<AuthDto> CreateUserAsync(SignUpDto signUpDto)
    {
        var newUser = new User
        {
            Email = signUpDto.Email,
            FirstName = signUpDto.FirstName,
            LastName = signUpDto.LastName,
            UserName = string.Concat(signUpDto.FirstName, signUpDto.LastName)
        };
        
        var result = await _userManager.CreateAsync(newUser, signUpDto.Password);

        if (!result.Succeeded)
        {
            string errors = string.Join(",", 
                result.Errors.Select(x => x.Description));

            throw new SignUpFailedException(errors);
        }

        return new AuthDto
        {
            AccessToken = _jwtService.GenerateToken(newUser.Id, newUser.UserName)
        };
    }

    public async Task<AuthDto> LoginUserAsync(SignInDto signInDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == signInDto.Email);

        if (user == null)
            throw new SignInFailedException("User not found");
        
        var passwordIsValid = await _userManager.CheckPasswordAsync(user, signInDto.Password);
        
        if (!passwordIsValid)
            throw new SignInFailedException("Invalid password");

        return new AuthDto
        {
            AccessToken = _jwtService.GenerateToken(user.Id, signInDto.Password)
        };
    }
}