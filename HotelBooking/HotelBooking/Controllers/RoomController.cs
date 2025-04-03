using HotelBooking.DTO.RequestDto;
using HotelBooking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers;

[ApiController]
[Route("rooms")]
public class RoomController : Controller
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAllRoomsAsync()
    {
        var rooms = await _roomService.GetAllAsync();
        
        return Ok(rooms);
    }
    
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoomDto>> GetRoomAsync([FromRoute]int id)
    {
        var room = await _roomService.GetByIdAsync(id);
        
        return Ok(room);
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateRoomAsync([FromBody]RoomDto roomDto)
    {
        await _roomService.CreateAsync(roomDto);

        return Ok("Room created");
    }
    
    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRoomAsync([FromRoute]int id, [FromBody]RoomDto roomDto)
    {
        await _roomService.UpdateAsync(id, roomDto);
        
        return Ok("Room updated");
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRoomAsync([FromRoute] int id)
    {
        await _roomService.DeleteAsync(id);
        
        return Ok("Room deleted");
    }
}