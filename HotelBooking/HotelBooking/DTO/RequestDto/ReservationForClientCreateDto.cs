using HotelBooking.DTO.RequestDto;

namespace HotelBooking.DTO.ResponseDto;

public class ReservationForClientCreateDto
{
    public DateTime CheckInDate { get; set; }
    
    public DateTime CheckOutDate { get; set; }
    
    public byte GuestCount { get; set; }

    public string? Description { get; set; }
    
    public List<ReservationRoomTypeDto> RoomTypes { get; set; } =  new List<ReservationRoomTypeDto>();
}