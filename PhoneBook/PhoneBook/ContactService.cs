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

    public ContactService()
    {
        _contacts = new List<Contact>();
    }
    
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
            throw new Exception("Contact with this name already exists");

        if (_contacts.Any(c => c.Number == phoneNumber))
            throw new Exception("Contact with this phone number already exists");

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
            throw new Exception("Contact with this name does not exist");
        
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
            throw new Exception("Contact not found");

        if (_contacts.Any(c => c.Name == newName && c.Id != id))
            throw new Exception("Contact with this name already exists");
        
        if (_contacts.Any(c => c.Number == newPhoneNumber && c.Id != id))
            throw new Exception("Contact with this phone number already exists");
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
            throw new Exception("Contact not found");

        _contacts.Remove(contact);

        return _contacts;
    }
}