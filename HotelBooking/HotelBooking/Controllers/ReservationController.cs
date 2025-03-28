using HotelBooking.DTO.RequestDto;
using HotelBooking.DTO.ResponseDto;
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
    
    [HttpGet("admin/reservations")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<ReservationForAdminDto>>> GetAllReservationsAsync()
    {
        var reservations = await _reservationService.GetAllWithRoomTypesAsync();
        
        return Ok(reservations);
    }
    
    [HttpGet("admin/reservations/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ReservationForAdminDto>> GetReservationByIdAsync([FromRoute]int id)
    {
        var reservation = await _reservationService.GetByIdWithRoomTypesAsync(id);
        
        return Ok(reservation);
    }
    
    [HttpGet("users/current/reservations")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<IEnumerable<ReservationForClientDto>>> GetAllCurrentReservationsAsync()
    {
        var reservations = await _reservationService.GetAllCurrentAsync(User);
        
        return Ok(reservations);
    }
    
    [HttpGet("users/current/reservations/{id}")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<ReservationForClientDto>> GetCurrentReservationByIdAsync([FromRoute]int id)
    {
        var reservation = await _reservationService.GetCurrentByIdAsync(id, User);
        
        return Ok(reservation);
    }

    [HttpPost("admin/reservations")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateReservationAsync([FromBody] ReservationForAdminDto reservation)
    {
        await _reservationService.CreateAsync(reservation);
        
        return Ok("Reservation created");
    }
    
    [HttpPost("users/current/reservations")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> CreateCurrentReservationAsync([FromBody]ReservationForClientCreateDto reservation)
    {
       await _reservationService.CreateCurrentAsync(User, reservation);
       
       return Ok("Reservation created");
    }
    
    [HttpPatch("admin/reservations/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateReservationAsync([FromRoute]int id, [FromBody]ReservationForAdminDto reservation)
    {
        await _reservationService.UpdateAsync(id, reservation);
        
        return Ok("Reservation updated");
    }
    
    [HttpPatch("users/current/reservations/{id}")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> UpdateCurrentReservationAsync([FromRoute] int id, [FromBody]ReservationForClientUpdateDto reservation)
    {
        await _reservationService.UpdateCurrentAsync(id, reservation, User);
        
        return Ok("Reservation updated");
    }
    
    [HttpDelete("admin/reservations/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteReservationAsync([FromRoute] int id)
    {
        await _reservationService.DeleteAsync(id);
        
        return Ok("Reservation deleted");
    }
    
    [HttpDelete("users/current/reservations/{id}")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> DeleteCurrentReservationAsync([FromRoute] int id)
    {
        await _reservationService.DeleteCurrentAsync(id, User);
        
        return Ok("Reservation deleted");
    }
}