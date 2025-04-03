using PhoneBook.Exceptions;

namespace PhoneBook;

interface IContactService
{
    List<Contact> GetContacts();
    List<Contact> AddContact(string name, string phoneNumber);
    Contact GetContactByName(string name);
    Contact GetContactByPhone(string phoneNumber);
    List<Contact> UpdateContact(int id, string newName, string newPhoneNumber);
    List<Contact> DeleteContact(int id);
}

public class ContactService : IContactService
{
    private readonly IFileManager _fileManager;
    private List<Contact> _contacts;

   public ContactService(FileManager fileManager)
    {
        _fileManager = fileManager;
        _contacts = _fileManager.GetContacts();
    }

    //Generate id by the last existing id in the list
    private int GenerateId()
    {
        if (_contacts.Count == 0)
            return 1;

        return _contacts.Max(c => c.Id) + 1;
    }

    public List<Contact> GetContacts() => _contacts;

    public List<Contact> AddContact(string name, string phoneNumber)
    {
        if (_contacts.Any(c => c.Name == name))
            throw new ContactNameExistsException();

        if (_contacts.Any(c => c.Number == phoneNumber))
            throw new ContactNumberExistsException();

        _contacts.Add(new Contact
        {
            Id = GenerateId(),
            Name = name,
            Number = phoneNumber,
        });

        return _contacts;
    }

    public Contact GetContactByName(string name)
    {
        var contact = _contacts.FirstOrDefault(c => c.Name == name);
        if (contact == null)
            throw new ContactNameNotFoundException();
        
        return contact;
    }

    public Contact GetContactByPhone(string phoneNumber)
    {
        return _contacts.FirstOrDefault(c => c.Number == phoneNumber);
    }

    public List<Contact> UpdateContact(int id, string newName, string newPhoneNumber)
    {
        var contact = _contacts.FirstOrDefault(c => c.Id == id);

        if (contact == null)
            throw new ContactNotFoundException();;

        if (_contacts.Any(c => c.Name == newName && c.Id != id))
            throw new ContactNameExistsException();
        
        if (_contacts.Any(c => c.Number == newPhoneNumber && c.Id != id))
            throw new ContactNumberExistsException();
        {

            contact.Name = newName;
            contact.Number = newPhoneNumber;

            return _contacts;
        }
    }
    
    public List<Contact> DeleteContact(int id)
    {
        var contact = _contacts.FirstOrDefault(c => c.Id == id);

        if (contact == null)
            throw new ContactNotFoundException();

        _contacts.Remove(contact);

        return _contacts;
    }
}