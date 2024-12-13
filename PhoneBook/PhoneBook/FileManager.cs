using System.Text.Json;
using PhoneBook.Exceptions;

namespace PhoneBook;

public interface IFileManager
{
    bool FileExists();
    List<Contact> GetContacts();
    void AddContacts(List<Contact> contacts);
}

public class FileManager : IFileManager
{
    private readonly string _filePath = "PhoneBook.json";

    public bool FileExists() => File.Exists(_filePath);

    public List<Contact> GetContacts()
    {
        if (!FileExists())
            throw new PhoneBookFileNotFoundException();
        
        var phoneBookJson = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Contact>>(phoneBookJson);
    }

    public void AddContacts(List<Contact> contacts)
    {
        if (contacts == null)
            throw new Exception("Contacts list cannot be saved as it's empty\n");
        
        var json = JsonSerializer.Serialize(contacts);
        File.WriteAllText(_filePath, json);
    }
}