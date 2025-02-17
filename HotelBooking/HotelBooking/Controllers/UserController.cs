using HotelBooking.DTO.ResponseDto;
using HotelBooking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers;

[ApiController]
[Route("users")]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    // GET
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> GetAllUsersAsync()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }
    
    //Get
    [HttpGet("{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> GetUserAsync(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        
        return Ok(user);
    }
    
    //Patch
    [HttpPatch("{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserAsync([FromRoute] int userId, [FromBody]UserDto userDto)
    {
        await _userService.PatchUserAsync(userId, userDto);
        
        return Ok("User updated");
    }
    
    //Delete
    [HttpDelete("{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUserAsync([FromRoute] int userId)
    {
        await _userService.DeleteUserAsync(userId);
        
        return Ok("User deleted");
    }

}