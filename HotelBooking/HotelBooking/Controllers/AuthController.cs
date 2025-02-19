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
    
    // POST
    [HttpPost("signup")]
    public async Task<ActionResult<AuthDto>> SignUp([FromBody]SignUpDto signUpDto)
    {
        bool isAdmin = false;
        var result = await _userService.CreateUserAsync(signUpDto, isAdmin);
        return Ok(result);
    }
    
    //POST
    [HttpPost("signin")]
    public async Task<ActionResult<AuthDto>> SignIn([FromBody] SignInDto signInDto)
    {
        var result = await _userService.LoginUserAsync(signInDto);
        return Ok(result);
    }
    
    //POST
    [HttpPost("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AuthDto>> CreateAdmin([FromBody] SignUpDto signUpDto)
    {
        bool isAdmin = true;
        var result = await _userService.CreateUserAsync(signUpDto, isAdmin);
        return Ok(result);
    }
}
