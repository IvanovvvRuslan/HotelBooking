﻿using HotelBooking.DTO.ResponseDto;
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
    [AllowAnonymous]
    [HttpGet]
    public async Task <ActionResult<IEnumerable<RoomTypeDto>>> GetAllRoomTypesAsync()
    {
        var roomTypes = await _roomTypeService.GetAllRoomTypesAsync();
        return Ok(roomTypes);
    }
    
    [AllowAnonymous]
    //GetById
    [HttpGet("{id}")]
    public async Task<ActionResult<RoomTypeDto>> GetRoomTypeByIdAsync([FromRoute]int id)
    {
        var roomType = await _roomTypeService.GetRoomTypeByIdAsync(id);
        return Ok(roomType);
    }
    
    //Post
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateRoomTypeAsync([FromBody]RoomTypeDto roomTypeDto)
    {
        await _roomTypeService.CreateRoomTypeAsync(roomTypeDto);
        return Ok(roomTypeDto);
    }
    
    //Patch
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateRoomTypeAsync([FromRoute]int id, [FromBody]RoomTypeDto roomTypeDto)
    {
        await _roomTypeService.PatchRoomTypeAsync(id, roomTypeDto);
        
        return Ok("Room Type Updated");
    }
    
    //Delete
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoomTypeAsync([FromRoute] int id)
    {
        await _roomTypeService.DeleteRoomTypeAsync(id);
        
        return Ok("Room Type Deleted");
    }
}