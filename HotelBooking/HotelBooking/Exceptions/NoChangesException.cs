namespace HotelBooking.Exceptions;

public class NoChangesException : Exception
{
    public NoChangesException(string message) : base(message) {}
}