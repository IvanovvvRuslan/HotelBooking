﻿using HotelBooking.DTO;
using HotelBooking.DTO.ResponseDto;
using HotelBooking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers;

[ApiController]
public class ClientController : Controller
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }
    
    // GET
    [HttpGet("admin/clients")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<ClientForAdminDto>>> GetAllClientsAsync()
    {
        var clients = await _clientService.GetAllAsync();
        
        return Ok(clients);
    }
    
    //GetById
    [HttpGet("admin/clients/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ClientForAdminDto>> GetClientByIdAsync([FromRoute]int id)
    {
        var client = await _clientService.GetByIdAsync(id);
        
        return Ok(client);
    }
    
    //GetCurrent
    [HttpGet("users/current/client")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<ClientForAdminDto>> GetCurrentClientAsync()
    {
        var client = await _clientService.GetCurrentAsync(User);
        
        return Ok(client);
    }
    
    //Post
    [HttpPost("admin/clients")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateClientAsync(SignUpDto signUpDto)
    {
        await _clientService.CreateAsync(signUpDto);
        
        return Ok("Client created");
    }
    
    //Patch
    [HttpPatch("admin/clients/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateClientAsync([FromRoute]int id, [FromBody]ClientForAdminDto clientForAdminDto)
    {
        await _clientService.UpdateAsync(id, clientForAdminDto);
        
        return Ok("Client updated");
    }
    
    //PatchCurrent
    [HttpPatch("users/current/client")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> UpdateCurrentClientAsync([FromBody] ClientForUserDto clientForUserDto)
    {
        await _clientService.UpdateCurrentAsync(User, clientForUserDto);
        
        return Ok("Account updated");
    }
    
    //Delete
    [HttpDelete("admin/clients/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteClientAsync([FromRoute] int id)
    {
        await _clientService.DeleteWithUserAsync(id);
        
        return Ok("Client deleted");
    }
    
    //DeleteCurrent
    [HttpDelete("users/current/client")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> DeleteCurrentClientAsync()
    {
        await _clientService.DeleteCurrentWithUserAsync(User);
        
        return Ok("Account deleted");
    }
}