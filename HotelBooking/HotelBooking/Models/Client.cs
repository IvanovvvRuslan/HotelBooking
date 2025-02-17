namespace HotelBooking.Models;

public class Client
{
    public int Id { get; set; }
    
    public int UserId { get; set; }

    public DateTime RegistrationDate { get; set; }

    public string? Gender { get; set; }
    
    public string? Country { get; set; }
    
    public bool IsVip { get; set; }
    
    public User User { get; set; } = null!;

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}