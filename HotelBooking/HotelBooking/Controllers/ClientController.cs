using HotelBooking.DTO.ResponseDto;
using HotelBooking.Models;
using HotelBooking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers;

[ApiController]
[Route("clients")]
public class ClientController : Controller
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }
    
    // GET
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetAllClientsAsync()
    {
        var clients = await _clientService.GetAllClientsAsync();
        
        return Ok(clients);
    }
    
    //GetById
    [HttpGet("{id}")]
    [Authorize(Policy = "ClientOrAdmin")]
    public async Task<ActionResult<ClientDto>> GetById([FromRoute]int id)
    {
        var client = await _clientService.GetClientById(id);
        
        return Ok(client);
    }
    
    //Post
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAsync(ClientDto clientDto)
    {
        await _clientService.CreateClientAsync(clientDto);
        
        return Ok("Client created");
    }
    
    //Patch
    [HttpPatch("{id}")]
    [Authorize(Policy = "ClientOrAdmin")]
    public async Task<IActionResult> UpdateAsync([FromRoute]int id, [FromBody]ClientDto clientDto)
    {
        await _clientService.UpdateClientAsync(id, clientDto);
        
        return Ok("Client updated");
    }
    
    //Delete
    [HttpDelete("{id}")]
    [Authorize(Policy = "ClientOrAdmin")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        await _clientService.DeleteClientAsync(id);
        
        return Ok("Client deleted");
    }
}