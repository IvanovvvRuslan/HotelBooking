using HotelBooking.DTO.RequestDto;
using HotelBooking.DTO.ResponseDto;
using HotelBooking.Models;
using HotelBooking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers;

[ApiController]
[Route("reservations")]
public class ReservationController : Controller
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }
    
    // GET
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<ReservationForAdminDto>>> GetAllReservationsAsync()
    {
        var reservations = await _reservationService.GetAllReservationsAsync();
        
        return Ok(reservations);
    }
    
    //GetById
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ReservationForAdminDto>> GetReservationByIdAsync([FromRoute]int id)
    {
        var reservation = await _reservationService.GetReservationByIdAsync(id);
        
        return Ok(reservation);
    }
    
    //GetAllCurrent
    [HttpGet("current")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<IEnumerable<ReservationForClientDto>>> GetAllCurrentReservationsAsync()
    {
        var reservations = await _reservationService.GetAllCurrentReservationsAsync(User);
        
        return Ok(reservations);
    }
    
    //GetCurrent
    [HttpGet("current/{id}")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<ReservationForClientDto>> GetCurrentReservationByIdAsync([FromRoute]int id)
    {
        var reservation = await _reservationService.GetCurrentReservationAsync(id, User);
        
        return Ok(reservation);
    }

    //Post
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateReservationAsync([FromBody] ReservationForAdminDto reservation)
    {
        await _reservationService.CreateReservationAsync(reservation);
        
        return Ok("Reservation created");
    }
    
    //PostCurrent
    [HttpPost("current")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> CreateCurrentReservationAsync([FromBody]ReservationForClientCreateDto reservation)
    {
       await _reservationService.CreateCurrentReservationAsync(User, reservation);
       
       return Ok("Reservation created");
    }
    
    //Patch
    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateReservationAsync([FromRoute]int id, [FromBody]ReservationForAdminDto reservation)
    {
        await _reservationService.UpdateReservationAsync(id, reservation);
        
        return Ok("Reservation updated");
    }
    
    //PatchCurrent
    [HttpPatch("current/{id}")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> UpdateCurrentReservationAsync([FromRoute] int id, [FromBody]ReservationForClientUpdateDto reservation)
    {
        await _reservationService.UpdateCurrentReservationAsync(id, reservation, User);
        
        return Ok("Reservation updated");
    }
    
    //Delete
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteReservationAsync([FromRoute] int id)
    {
        await _reservationService.DeleteReservationAsync(id);
        
        return Ok("Reservation deleted");
    }
    
    //DeleteCurrent
    [HttpDelete("current/{id}")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> DeleteCurrentReservationAsync([FromRoute] int id)
    {
        await _reservationService.DeleteCurrentReservationAsync(id, User);
        
        return Ok("Reservation deleted");
    }
}