namespace HotelBooking.Models;

public class ReservationRoomType
{
    public int ReservationId { get; set; }

    public int RoomTypeId { get; set; }
    
    public Reservation Reservation { get; set; }
    
    public RoomType RoomType { get; set; }
}