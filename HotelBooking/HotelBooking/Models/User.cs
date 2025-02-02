using Microsoft.AspNetCore.Identity;

 namespace HotelBooking.Models;

public class User : IdentityUser<int>
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string? Description { get; set; }

    public Admin Admin { get; set; }
    public Client Client { get; set; }
}