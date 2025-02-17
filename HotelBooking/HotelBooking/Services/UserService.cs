using HotelBooking.Data;
using HotelBooking.DTO;
using HotelBooking.DTO.ResponseDto;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using HotelBooking.Repositories;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Services;

public interface IUserService
{
    Task<AuthDto> CreateUserAsync(SignUpDto signUpDto, bool isAdmin);
    Task<AuthDto> LoginUserAsync(SignInDto signInDto);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto> GetUserByIdAsync(int id);
    Task PatchUserAsync(int id, UserDto userDto);
    Task DeleteUserAsync(int id);
}

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;
    private readonly ApplicationDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IAdminRepository _adminRepository;
    private readonly IClientRepository _clientRepository;

    public UserService(UserManager<User> userManager, IJwtService jwtService, ApplicationDbContext context,
        IUserRepository repository, IAdminRepository adminRepository, IClientRepository clientRepository)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _context = context;
        _userRepository = repository;
        _adminRepository = adminRepository;
        _clientRepository = clientRepository;
    }
    
    public async Task<AuthDto> CreateUserAsync(SignUpDto signUpDto, bool isAdmin)
    {
        var existingUser = await _userManager.FindByEmailAsync(signUpDto.Email);
        if (existingUser != null)
            throw new SignUpFailedException("User with this email already exists");
        
        var newUser = new User
        {
            Email = signUpDto.Email,
            FirstName = signUpDto.FirstName,
            LastName = signUpDto.LastName,
            UserName = string.Concat(signUpDto.FirstName, signUpDto.LastName)
        };
        
        var result = await _userManager.CreateAsync(newUser, signUpDto.Password);
        
        var role = isAdmin ? "Admin" : "Client";
        
        //Add new User to Client/Admin context
        if (isAdmin)
        {
            var newAdmin = new Admin
            {
                UserId = newUser.Id,
            };
            await _adminRepository.CreateAsync(newAdmin);
        }
        else
        {
            var newClient = new Client
            {
                UserId = newUser.Id,
                RegistrationDate = DateTime.UtcNow,
                IsVip = false,
            };
            await _clientRepository.CreateAsync(newClient);
        }
        
        await _userManager.AddToRoleAsync(newUser, role);

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

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();

        var usersDto = users.Adapt<IEnumerable<UserDto>>();

        return usersDto;
    }

    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
            throw new NotFoundException("User not found");

        var userDto = user.Adapt<UserDto>();
        
        return userDto;
    }

    public async Task PatchUserAsync(int id, UserDto userDto)
    {
        var userOld = await _userRepository.GetByIdTrackedAsync(id);

        if (userOld == null)
            throw new NotFoundException("User not found");
        
        userOld.FirstName = userDto.FirstName;
        userOld.LastName = userDto.LastName;
        userOld.Email = userDto.Email;
        userOld.Description = userDto.Description;
        userOld.UserName = userDto.UserName;
        userOld.PhoneNumber = userDto.PhoneNumber;
        
        await _userRepository.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetByIdTrackedAsync(id);

        if (user == null)
            throw new NotFoundException("User not found");
        
        await _userRepository.DeleteAsync(user);
        await _userRepository.SaveChangesAsync();
    }
}