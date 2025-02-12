using System.Globalization;
using System.Resources;

namespace Task1;

public class Localization
{
    private ResourceManager _resourceManager = new ResourceManager("Task1.Resources.Messages", typeof(Localization).Assembly);
    public string SelectedLanguage { get; set; } = "ru";

    public void SetLanguage(InputOutputHandler ioHandler)
    {
        bool isCommand;
        ioHandler.DisplayMessage("Choose language / Выберите язык:");
        ioHandler.DisplayMessage("1. English");
        ioHandler.DisplayMessage("2. Русский");

        string choice = ioHandler.GetInput(out isCommand);
        SelectedLanguage = choice == "1" ? "en" : "ru";
    }
    
    public string GetMessage(string messageKey)
    {
        CultureInfo culture = SelectedLanguage == "en" ? CultureInfo.CreateSpecificCulture("en") : CultureInfo.InvariantCulture;
        return _resourceManager.GetString(messageKey, culture) ?? messageKey;
    }
}