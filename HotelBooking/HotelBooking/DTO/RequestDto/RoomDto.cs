using HotelBooking.Models;

namespace HotelBooking.DTO.RequestDto;

public class RoomDto
{
    public int RoomTypeId { get; set; }
    public string RoomNumber { get; set; }
    public string Description { get; set; }
}