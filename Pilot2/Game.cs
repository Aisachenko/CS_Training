using Newtonsoft.Json;

namespace Task1;

public class Game
{
    private string _baseWord;
    bool isCommand;
    private int _currentPlayerIndex = 0;
    private List<Player> currentPlayers = new List<Player>();
    private WordValidator _wordValidator = new WordValidator();
    private Localization _localization = new Localization();
    private InputOutputHandler ioHandler = new InputOutputHandler();
    static string fileName = "/gameData.json";
    string filePath = Environment.CurrentDirectory + fileName;
    

    private void EnterPlayerName(out string playerName, int n)
    {
        while (true)
        {
            ioHandler.DisplayMessage(_localization.GetMessage("EnterPlayerName"), n);
            playerName = ioHandler.GetInput(out isCommand);
            if (isCommand)
            {
                ProcessCommand(playerName);
            }
            else
            {
                AddPlayer(playerName);
                break;
            }
        }
    }

    public void StartGame()
    {
        _localization.SetLanguage(ioHandler);
        string primaryWord;

        ReadFromFile();
        for (int i = 0; i < 2; i++)
        {
            Player player = new Player();
            EnterPlayerName(out player.Name, i + 1);
            currentPlayers.Add(player);
        }

        while (true)
        {
            ioHandler.DisplayMessage(_localization.GetMessage("EnterWordPrompt"));
            primaryWord = ioHandler.GetInput(out isCommand);
            if (isCommand)
            {
                ProcessCommand(primaryWord);
            }
            else if (!_wordValidator.IsValidPrimaryWord(primaryWord))
            {
                ioHandler.DisplayMessage(_localization.GetMessage("TryAgainPrompt"));
            }
            else break;
        }

        _wordValidator.SetAvailableLetters(primaryWord);
        ioHandler.DisplayMessage(_localization.GetMessage("PrimaryWordSuccess"), primaryWord);
        PlayGame();
    }

    private void PlayGame()
    {
        int currentPlayer = 0;
        int count = 0;

        while (true)
        {
            ioHandler.DisplayMessage(_localization.GetMessage("PlayerEnter"), currentPlayers[currentPlayer].Name);
            (bool timeOut, string word) = ioHandler.GetTimedInput(out isCommand);
            if (isCommand)
            {
                ProcessCommand(word);
            }
            else
            {
                if (timeOut)
                {
                    ioHandler.DisplayMessage(_localization.GetMessage("TimeOut"));
                    return;
                }

                (bool isValid, string info) = _wordValidator.IsValidWord(word);

                if (isValid)
                {
                    ioHandler.DisplayMessage(_localization.GetMessage("PlayerEntered"), currentPlayers[currentPlayer].Name, word);
                    currentPlayer = currentPlayer == 0 ? 1 : 0;
                    count++;
                }
                else
                {
                    ioHandler.DisplayMessage(_localization.GetMessage("PlayerLoose"), currentPlayers[currentPlayer].Name, word,
                        info);
                    int winner = currentPlayer == 0 ? 1 : 0;
                    UpdatePlayer(currentPlayers[winner].Name, count / 2);
                    break;
                }
            }
        }
    }
    
    private void ProcessCommand(string command)
    {
        switch (command)
        {
            case "/help":
                ioHandler.DisplayMessage(_localization.GetMessage("ShowHelp"));
                break;
            case "/show-words":
                ioHandler.DisplayMessage(_localization.GetMessage("ShowWords"), _wordValidator.UsedWords());
                break;
            case "/score":
                currentPlayers = GetCurrentPlayers(currentPlayers[1].Name, currentPlayers[2].Name);
                foreach (Player player in currentPlayers)
                {
                    ioHandler.DisplayMessage(_localization.GetMessage("ShowScore"), player.Name, player.Score);
                }

                break;
            case "/total-score":
                ioHandler.DisplayMessage(_localization.GetMessage("ShowTotalScore"));
                List<Player> allPlayers = GetAllPlayers();
                foreach (Player player in allPlayers)
                {
                    ioHandler.DisplayMessage(_localization.GetMessage("ShowScore"), player.Name, player.Score);
                }

                break;
            case "/exit":
                Environment.Exit(0);
                break;
            default:
                ioHandler.DisplayMessage(_localization.GetMessage("UnknownCommand"));
                break;
        }
    }

    void AddPlayer(string name, int score = 0)
    {
        List<Player> allPlayers = ReadFromFile();
        if (allPlayers.FindAll(player => player.Name == name) != null)
        {
            Player player = new Player(name, score);
            allPlayers.Add(player);
            SaveToFile(allPlayers);
        }
    }

    void RemovePlayer(string name)
    {
        List<Player> allPlayers = ReadFromFile();
        Player player = allPlayers.FirstOrDefault(p => p.Name == name);
        if (player != null)
        {
            allPlayers.Remove(player);
            SaveToFile(allPlayers);
        }
    }

    public void UpdatePlayer(string name, int score)
    {
        RemovePlayer(name);
        AddPlayer(name, score);
    }

    public List<Player> GetCurrentPlayers(string playerName1, string playerName2)
    {
        List<Player> allPlayers = ReadFromFile();
        return allPlayers.FindAll(player => player.Name == playerName1 || player.Name == playerName2);
    }

    public List<Player> GetAllPlayers()
    {
        List<Player> allPlayers = ReadFromFile();
        return allPlayers;
    }

    public List<Player> ReadFromFile()
    {
        if (!File.Exists(filePath))
            return new List<Player>();

        string json = File.ReadAllText(filePath);

        if (string.IsNullOrWhiteSpace(json))
            return new List<Player>();
        if (json.TrimStart().StartsWith("{"))
            return new List<Player>();

        return JsonConvert.DeserializeObject<List<Player>>(json) ?? new List<Player>();
    }

    void SaveToFile(List<Player> players)
    {
        string serializedPlayers = JsonConvert.SerializeObject(players);
        File.WriteAllText(filePath, serializedPlayers);
    }
}