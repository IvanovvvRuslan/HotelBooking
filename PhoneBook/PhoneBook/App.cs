using System.Text.Json;

namespace PhoneBook;

public class App
{
    private readonly UI _ui;

    public App(UI ui)
    {
        _ui = ui;
    }
    
    public void AppStart()
    {
        while (true)
        {
            var userChoise = _ui.GetUserChoice();
            
            if (userChoise == 6) 
                break;
            
            _ui.GetService(userChoise);
        }
    }
}