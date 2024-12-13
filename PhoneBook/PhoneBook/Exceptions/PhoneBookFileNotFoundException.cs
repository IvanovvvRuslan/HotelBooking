namespace PhoneBook.Exceptions;

public class PhoneBookFileNotFoundException : Exception
{
    public PhoneBookFileNotFoundException() : base("Phone Book file doesn't exist") { }
    
    public PhoneBookFileNotFoundException(string message) : base(message) { }
}