namespace HotelBooking.Models;

public class ReservationRoomType
{
    public int ReservationId { get; set; }

    public int RoomTypeId { get; set; }
    
    public Reservation Reservation { get; set; } = null!;
    
    public RoomType RoomType { get; set; } = null!;

    public byte ReservedRoomCount { get; set; }

    public DateTime CreatedAt { get; set; }
}