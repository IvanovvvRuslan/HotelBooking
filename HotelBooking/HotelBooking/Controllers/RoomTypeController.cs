using HotelBooking.DTO.ResponseDto;
using HotelBooking.Services;
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
    public async Task <ActionResult<IEnumerable<RoomTypeDto>>> GetAllRoomTypes()
    {
        var roomTypes = await _roomTypeService.GetAllRoomTypesAsync();
        return Ok(roomTypes);
    }
    
    //GetById
    [HttpGet("{id}")]
    public async Task<ActionResult<RoomTypeDto>> GetRoomType([FromRoute]int id)
    {
        var roomType = await _roomTypeService.GetRoomTypeByIdAsync(id);
        return Ok(roomType);
    }
    
    //Post
    [HttpPost]
    public async Task<IActionResult> CreateRoomType([FromBody]RoomTypeDto roomTypeDto)
    {
        await _roomTypeService.CreateRoomTypeAsync(roomTypeDto);
        return Ok(roomTypeDto);
    }
    
    //Patch
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateRoomType([FromRoute]int id, [FromBody]RoomTypeDto roomTypeDto)
    {
        await _roomTypeService.PatchRoomTypeAsync(id, roomTypeDto);
        
        return Ok("Room Type Updated");
    }
    
    //Delete
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoomType([FromRoute] int id)
    {
        await _roomTypeService.DeleteRoomTypeAsync(id);
        return Ok("Room Type Deleted");
    }
}