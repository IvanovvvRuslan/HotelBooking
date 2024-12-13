using System.Diagnostics;

namespace PhoneBook;

public class UI
{
    private readonly IContactService _contactService;
    private readonly IFileManager _fileManager;

    public UI(ContactService contactService, FileManager fileManager)
    {
        _contactService = contactService;
        _fileManager = fileManager;
    }
    
    //User menu
    public int GetUserChoice()
    {
        int userChoice = 0;
        bool validChoice = false;

        while (!validChoice)
        {
            DesplayMenu();
                  
            int.TryParse(Console.ReadLine(), out userChoice);

            if (userChoice >0 && userChoice < 7)
            {
                validChoice = true;
            }
            else
            {
                Console.WriteLine("Invalid choice. Please try again.");
            }
        }
        return userChoice; 
    }

    //User choice
    public void GetService(int userChoice)
    {
        switch (userChoice)
        {
            case 1:
                AddContact();
                break;
            
            case 2:
                SearchContactByName();
                break;
            
            case 3:
               GetAllContacts();
               break;
            
            case 4:
                UpdateContact();
                break;
            
            case 5:
                DeleteContact();
                break;
            
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }

    //Get list of all contacts
    public void GetAllContacts()
    {
        var contacts = _contactService.GetContacts();

        if (contacts == null || contacts.Count == 0)
        {
            Console.Clear();
            Console.WriteLine("There are no contacts in the file\n");
            return;
        }
        
        foreach (Contact contact in contacts)
        {
            Console.WriteLine($"Id: {contact.Id}, Name:  {contact.Name}, Number: {contact.Number}");
        };
        Console.WriteLine();
    }

    //Add contact
    public void AddContact()
    {
        Console.WriteLine("Enter contact name: ");
        var name = GetValidName();

        Console.WriteLine("Enter contact number: ");
        var number = GetValidNumber();
        
        try
        {
            var contacts = _contactService.AddContact(name, number);
            _fileManager.AddContacts(contacts);

            Console.WriteLine("\nContact added\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    //Search contact by name
    public void SearchContactByName()
    {
        Console.WriteLine("Enter contact name: ");
        var name = GetValidName();
        
        var contact = TryGetContactByName(name);

        if (contact != null)
            Console.WriteLine($"\nId: {contact.Id}, Name: {contact.Name}, Number: {contact.Number}\n");
    }

    //Edit contact
    public void UpdateContact()
    {
        Console.WriteLine("Enter contact name: ");
        var name = GetValidName();
        
        Console.WriteLine("\nPlese enter the new contact name to update:");
        var newName = GetValidName();
        
        Console.WriteLine("\nPlease enter the new contact number:");
        var newNumber = GetValidNumber();

        var contact = TryGetContactByName(name);

        if (contact != null)
        {
            var updatedContacts = _contactService.UpdateContact(contact.Id, newName, newNumber);
            _fileManager.AddContacts(updatedContacts);
        }
    }

    //Delete contact
    public void DeleteContact()
    {
        Console.WriteLine("Enter contact name to delete: ");
        var name = GetValidName();
        
        var contact = TryGetContactByName(name);

        if (contact != null)
        {
            Console.WriteLine($"Contact found: \nName: {contact.Name}, Number: {contact.Number}\n");
            
            var updatedContacts = _contactService.DeleteContact(contact.Id);
            _fileManager.AddContacts(updatedContacts);
        
            Console.WriteLine("Contact deleted\n");
        }
    }

    //Menu items
    private void DesplayMenu()
    {
        Console.WriteLine("Please choose an action:");
        Console.WriteLine("1. Add Contact");
        Console.WriteLine("2. Search by name");
        Console.WriteLine("3. Get full contact list");
        Console.WriteLine("4. Update contact");
        Console.WriteLine("5. Delete contact");
        Console.WriteLine("6. Exit");
    }

    //Name input validation
    private string GetValidName()
    {
        string name;
        do
        {
            name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("\nName cannot be empty. Please try again\n");
            }
        } while (string.IsNullOrWhiteSpace(name));
        
        return name;
    }
    
    //Phone number input validation
    private string GetValidNumber()
    {
        string number;
        do
        {
            number = Console.ReadLine();
            if (!System.Text.RegularExpressions.Regex.IsMatch(number, @"^\+?\d+$"))
            {
                Console.WriteLine("Number must contain only digits and and optionally starts with +.\n" +
                                  "For example: +380671234567\nPlease try again");
            }
        } while (!System.Text.RegularExpressions.Regex.IsMatch(number, @"^\+?\d+$"));
        
        return number;
    }

    //Search for a contact by name using try-catch
    private Contact TryGetContactByName(string name)
    {
        try
        {
            var contact = _contactService.GetContactByName(name);
            return contact;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }
}
