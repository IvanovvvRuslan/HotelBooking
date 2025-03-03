using HotelBooking.DTO.ResponseDto;
using HotelBooking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers;

[ApiController]
[Route("roomtypes")]
public class RoomTypeController : ControllerBase
{
    private readonly IRoomTypeService _roomTypeService;

    public RoomTypeController(IRoomTypeService roomTypeService)
    {
        _roomTypeService = roomTypeService;
    }
    
    // GET
    [HttpGet]
    [AllowAnonymous]
    public async Task <ActionResult<IEnumerable<RoomTypeDto>>> GetAllRoomTypesAsync()
    {
        var roomTypes = await _roomTypeService.GetAllAsync();
        
        return Ok(roomTypes);
    }
    
    
    //GetById
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<RoomTypeDto>> GetRoomTypeByIdAsync([FromRoute]int id)
    {
        var roomType = await _roomTypeService.GetByIdAsync(id);
        
        return Ok(roomType);
    }
    
    //Post
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateRoomTypeAsync([FromBody]RoomTypeDto roomTypeDto)
    {
        await _roomTypeService.CreateAsync(roomTypeDto);
        
            return Ok("Room Type created");
    }
    
    //Patch
    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRoomTypeAsync([FromRoute]int id, [FromBody]RoomTypeDto roomTypeDto)
    {
        await _roomTypeService.UpdateAsync(id, roomTypeDto);
        
        return Ok("Room Type Updated");
    }
    
    //Delete
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRoomTypeAsync([FromRoute]int id)
    {
        await _roomTypeService.DeleteAsync(id);
        
        return Ok("Room Type Deleted");
    }
}