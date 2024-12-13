namespace PhoneBook.Exceptions;

public class ContactNameNotFoundException : Exception
{
    public ContactNameNotFoundException() : base("Contact with this name does not exist") {}

    public ContactNameNotFoundException(string message) : base(message) {}
}