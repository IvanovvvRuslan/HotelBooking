using System.Threading.Channels;
using PhoneBook;
using PhoneBook.Exceptions;

Console.WriteLine("PhoneBook App Started\n");

var fileManager = new FileManager();
ContactService contactService = null;

try
{
    contactService = new ContactService(fileManager);
}
catch (PhoneBookFileNotFoundException ex)
{
    contactService = new ContactService();
    Console.WriteLine(ex.Message);
}
catch (Exception ex)
{
    Console.WriteLine($"Something went wrong: {ex.Message}");
}

var ui = new UI(contactService, fileManager);
var app = new App(ui);
app.AppStart();







