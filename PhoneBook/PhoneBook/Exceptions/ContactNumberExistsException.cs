namespace PhoneBook.Exceptions;

public class ContactNumberExistsException : Exception
{
    public ContactNumberExistsException() : base("Contact with this phone number already exists") {}

    public ContactNumberExistsException(string message) : base(message) {}
}