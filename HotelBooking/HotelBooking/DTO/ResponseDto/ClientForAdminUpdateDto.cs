namespace HotelBooking.DTO.ResponseDto;

public class ClientForAdminUpdateDto
{
    public string PhoneNumber { get; set; }
    
    public string? Gender { get; set; }
    
    public string? Country { get; set; }
    
    public bool isVip { get; set; } = false;
}