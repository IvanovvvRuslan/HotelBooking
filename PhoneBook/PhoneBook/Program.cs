using System.Threading.Channels;
using PhoneBook;
using PhoneBook.Exceptions;

Console.WriteLine("PhoneBook App Started\n");

var fileManager = new FileManager();
var contactService = new ContactService(fileManager);
var ui = new UI(contactService, fileManager);
var app = new App(ui);
app.AppStart();







