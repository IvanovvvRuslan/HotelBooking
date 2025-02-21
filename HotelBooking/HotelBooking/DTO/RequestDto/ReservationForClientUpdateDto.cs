namespace HotelBooking.DTO.RequestDto;

public class ReservationForClientUpdateDto
{
    public DateTime CheckInDate { get; set; }
    
    public DateTime CheckOutDate { get; set; }
    
    public byte GuestCount { get; set; }

    public string? Description { get; set; }
}