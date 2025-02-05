namespace HotelBooking.DTO.ResponseDto;

public class RoomTypeDto
{
    public string Name { get; set; }
    
    public decimal Price { get; set; }
    
    public byte MaxOccupancy { get; set; }
    
    public string? Description { get; set; }
}