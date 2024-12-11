using System.Text.Json;

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
            throw new Exception("Phonebook file doesn't exist\n");
        
        try
        {
            var phoneBookJson = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Contact>>(phoneBookJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"File read error {ex.Message}");
            return new List<Contact>();
        }
    }

    public void AddContacts(List<Contact> contacts)
    {
        if (contacts == null || contacts.Count == 0)
            throw new Exception("Contacts list cannot be saved as it's empty\n");
        
        var json = JsonSerializer.Serialize(contacts);
        File.WriteAllText(_filePath, json);
    }
}