namespace HotelBooking.Models;

public class Room
{
    public int Id { get; set; }
    public int RoomTypeId { get; set; }
    public string RoomNumber { get; set; }
    public string Description { get; set; }

    public RoomType RoomType { get; set; } = null!;
}