namespace HotelBooking.Models;

public class Reservation
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string Status { get; set; }
    public decimal TotalPrice { get; set; }
    public byte GuestCount { get; set; }
    public string? Description { get; set; }
    public Client Client { get; set; } = null!;

    public ICollection<ReservationRoomType> RoomTypes { get; set; } = new List<ReservationRoomType>();
}