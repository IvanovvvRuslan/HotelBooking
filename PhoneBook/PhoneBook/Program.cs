using PhoneBook;

Console.WriteLine("PhoneBook App Started\n");

var fileManager = new FileManager();
var service = new Service(fileManager);
var menu = new UI(service, fileManager);
var app = new App(menu);
app.AppStart();
