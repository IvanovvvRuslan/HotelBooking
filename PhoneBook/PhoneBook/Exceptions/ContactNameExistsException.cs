namespace PhoneBook.Exceptions;

public class ContactNameExistsException : Exception
{
    public ContactNameExistsException() : base("Contact with this name already exists") {}

    public ContactNameExistsException(string message) : base(message) {}
}