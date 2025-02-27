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
        //var users = await _userService.GetAllUsersAsync();
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }
    
    //Get
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> GetUserByIdAsync(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        
        return Ok(user);
    }
    
    //Patch
    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserAsync([FromRoute] int id, [FromBody]UserDto userDto)
    {
        await _userService.UpdateUserAsync(id, userDto);
        
        return Ok("User updated");
    }
    
    //Delete
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUserAsync([FromRoute] int id)
    {
        await _userService.DeleteAsync(id);
        
        return Ok("User deleted");
    }

}