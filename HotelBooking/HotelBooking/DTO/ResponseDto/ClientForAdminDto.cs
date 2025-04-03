namespace HotelBooking.DTO.ResponseDto;

public class ClientForAdminDto
{
    public int UserId { get; set; }

    public string PhoneNumber { get; set; }
    
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    
    public string? Gender { get; set; }
    
    public string? Country { get; set; }
    
    public bool isVip { get; set; } = false;
}