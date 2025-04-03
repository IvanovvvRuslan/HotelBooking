using HotelBooking.DTO;
using HotelBooking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : Controller
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost("signup")]
    public async Task<ActionResult<AuthDto>> SignUpAsync([FromBody]SignUpDto signUpDto)
    {
        bool isAdmin = false;
        var result = await _userService.CreateAsync(signUpDto, isAdmin);
        return Ok(result);
    }
    
    [HttpPost("signin")]
    public async Task<ActionResult<AuthDto>> SignInAsync([FromBody] SignInDto signInDto)
    {
        var result = await _userService.LoginAsync(signInDto);
        return Ok(result);
    }
    
    [HttpPost("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AuthDto>> CreateAdminAsync([FromBody] SignUpDto signUpDto)
    {
        bool isAdmin = true;
        var result = await _userService.CreateAsync(signUpDto, isAdmin);
        return Ok(result);
    }
}
