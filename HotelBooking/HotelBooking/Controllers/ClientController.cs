using HotelBooking.DTO.ResponseDto;
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
    public async Task<ActionResult<IEnumerable<ClientForAdminDto>>> GetAllClientsAsync()
    {
        var clients = await _clientService.GetAllClientsAsync();
        
        return Ok(clients);
    }
    
    //GetById
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ClientForAdminDto>> GetClientByIdAsync([FromRoute]int id)
    {
        var client = await _clientService.GetClientByIdAsync(id);
        
        return Ok(client);
    }
    
    //GetCurrent
    [HttpGet("current")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<ClientForAdminDto>> GetCurrentClientAsync()
    {
        var client = await _clientService.GetCurrentClientAsync(User);
        
        return Ok(client);
    }
    
    //Post
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateClientAsync(ClientForAdminDto clientForAdminDto)
    {
        await _clientService.CreateClientAsync(clientForAdminDto);
        
        return Ok("Client created");
    }
    
    //Patch
    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateClientAsync([FromRoute]int id, [FromBody]ClientForAdminDto clientForAdminDto)
    {
        await _clientService.UpdateClientAsync(id, clientForAdminDto);
        
        return Ok("Client updated");
    }
    
    //PatchCurrent
    [HttpPatch("current")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> UpdateCurrentClientAsync([FromBody] ClientForUserDto clientForUserDto)
    {
        await _clientService.UpdateCurrentClientAsync(User, clientForUserDto);
        
        return Ok("Account updated");
    }
    
    //Delete
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteClientAsync([FromRoute] int id)
    {
        await _clientService.DeleteClientAsync(id);
        
        return Ok("Client deleted");
    }
    
    //DeleteCurrent
    [HttpDelete("current")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> DeleteCurrentClientAsync()
    {
        await _clientService.DeleteCurrentClientAsync(User);
        
        return Ok("Account deleted");
    }
}