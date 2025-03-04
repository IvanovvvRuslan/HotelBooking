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

public interface IUserService : IGenericService<User, UserDto>
{
    Task<AuthDto> CreateAsync(SignUpDto signUpDto, bool isAdmin);
    Task<AuthDto> LoginAsync(SignInDto signInDto);
    Task UpdateAsync(int id, UserDto userDto);
}

public class UserService : GenericService<User, UserDto>, IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;
    private readonly ApplicationDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IAdminRepository _adminRepository;
    private readonly IClientRepository _clientRepository;

    public UserService(UserManager<User> userManager, IJwtService jwtService, ApplicationDbContext context,
        IUserRepository repository, IAdminRepository adminRepository, IClientRepository clientRepository) : base(repository)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _context = context;
        _userRepository = repository;
        _adminRepository = adminRepository;
        _clientRepository = clientRepository;
    }
    
    public async Task<AuthDto> CreateAsync(SignUpDto signUpDto, bool isAdmin)
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
        
        if (!result.Succeeded && result == null)
        {
            string errors = string.Join(",", 
                result.Errors.Select(x => x.Description));

            Console.WriteLine("Creature failed" + errors);

            throw new SignUpFailedException(errors);
        }
        
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

        return new AuthDto
        {
            AccessToken = _jwtService.GenerateToken(newUser.Id, newUser.UserName)
        };
    }

    public async Task<AuthDto> LoginAsync(SignInDto signInDto)
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

    public async Task UpdateAsync(int id, UserDto userDto)
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
}