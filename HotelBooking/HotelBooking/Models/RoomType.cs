namespace HotelBooking.Models;

public class RoomType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public byte MaxOccupancy { get; set; }
    public string? Description { get; set; }

    public ICollection<ReservationRoomType> Reservations { get; set; } = new List<ReservationRoomType>();
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}