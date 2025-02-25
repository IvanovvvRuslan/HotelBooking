using HotelBooking.DTO.RequestDto;
using HotelBooking.DTO.ResponseDto;
using HotelBooking.Models;
using HotelBooking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers;

[ApiController]
public class ReservationController : Controller
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }
    
    // GET
    [HttpGet("admin/reservations")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<ReservationForAdminDto>>> GetAllReservationsAsync()
    {
        var reservations = await _reservationService.GetAllReservationsAsync();
        
        return Ok(reservations);
    }
    
    //GetById
    [HttpGet("admin/reservations/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ReservationForAdminDto>> GetReservationByIdAsync([FromRoute]int id)
    {
        var reservation = await _reservationService.GetReservationByIdAsync(id);
        
        return Ok(reservation);
    }
    
    //GetAllCurrent
    [HttpGet("users/current/reservations")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<IEnumerable<ReservationForClientDto>>> GetAllCurrentReservationsAsync()
    {
        var reservations = await _reservationService.GetAllCurrentReservationsAsync(User);
        
        return Ok(reservations);
    }
    
    //GetCurrent
    [HttpGet("users/current/reservations/{id}")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<ReservationForClientDto>> GetCurrentReservationByIdAsync([FromRoute]int id)
    {
        var reservation = await _reservationService.GetCurrentReservationAsync(id, User);
        
        return Ok(reservation);
    }

    //Post
    [HttpPost("admin/reservations")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateReservationAsync([FromBody] ReservationForAdminDto reservation)
    {
        await _reservationService.CreateReservationAsync(reservation);
        
        return Ok("Reservation created");
    }
    
    //PostCurrent
    [HttpPost("users/current/reservations")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> CreateCurrentReservationAsync([FromBody]ReservationForClientCreateDto reservation)
    {
       await _reservationService.CreateCurrentReservationAsync(User, reservation);
       
       return Ok("Reservation created");
    }
    
    //Patch
    [HttpPatch("admin/reservations/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateReservationAsync([FromRoute]int id, [FromBody]ReservationForAdminDto reservation)
    {
        await _reservationService.UpdateReservationAsync(id, reservation);
        
        return Ok("Reservation updated");
    }
    
    //PatchCurrent
    [HttpPatch("users/current/reservations/{id}")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> UpdateCurrentReservationAsync([FromRoute] int id, [FromBody]ReservationForClientUpdateDto reservation)
    {
        await _reservationService.UpdateCurrentReservationAsync(id, reservation, User);
        
        return Ok("Reservation updated");
    }
    
    //Delete
    [HttpDelete("admin/reservations/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteReservationAsync([FromRoute] int id)
    {
        await _reservationService.DeleteReservationAsync(id);
        
        return Ok("Reservation deleted");
    }
    
    //DeleteCurrent
    [HttpDelete("users/current/reservations/{id}")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> DeleteCurrentReservationAsync([FromRoute] int id)
    {
        await _reservationService.DeleteCurrentReservationAsync(id, User);
        
        return Ok("Reservation deleted");
    }
}