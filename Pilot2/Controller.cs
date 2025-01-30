namespace Task1
{
    public class Controller
    {
        private Model _model = new Model();
        private View _view = new View();
        string[] playerName = new string[2];
        bool isCommand;

        private void SetLanguage()
        {
            bool isCommand;
            _view.DisplayMessage("Choose language / Выберите язык:");
            _view.DisplayMessage("1. English");
            _view.DisplayMessage("2. Русский");

            string choice;
            while (true)
            {
                choice = _view.GetInput(out isCommand);
                if (!isCommand) break;
                ProcessCommand(choice);
            }
            _model.SelectedLanguage = choice == "1" ? "en" : "ru";
        }
        
        private void ProcessCommand(string command)
        {
            switch (command)
            {
                case "/help":
                    _view.DisplayMessage(_model.GetMessage("ShowHelp"));
                    break;
                case "/show-words":
                    _view.DisplayMessage(_model.GetMessage("ShowWords"), _model.UsedWords());
                    break;
                case "/score":
                    List<Player> currentPlayers = _model.GetCurrentPlayers(playerName[1], playerName[2]);
                    foreach (Player player in currentPlayers)
                    {
                        _view.DisplayMessage(_model.GetMessage("ShowScore"), player.Name, player.Score);
                    }
                    break;
                case "/total-score":
                    _view.DisplayMessage(_model.GetMessage("ShowTotalScore"));
                    List<Player> allPlayers = _model.GetAllPlayers();
                    foreach (Player player in allPlayers)
                    {
                        _view.DisplayMessage(_model.GetMessage("ShowScore"), player.Name, player.Score);
                    }
                    break;
                case "/exit":
                    Environment.Exit(0);
                    break;
                default:
                    _view.DisplayMessage(_model.GetMessage("UnknownCommand"));
                    break;
            }
        }

        private void EnterPlayerName(out string playerName, int n)
        {
            while (true)
            {
                _view.DisplayMessage(_model.GetMessage("EnterPlayerName"), n);
                playerName = _view.GetInput(out isCommand);
                if (isCommand)
                {
                    ProcessCommand(playerName);
                }
                else
                {
                    _model.AddPlayer(playerName);
                    break;
                }
            }
        }
        
        public void StartGame()
        {
            SetLanguage();
            string primaryWord;
            
            _model.ReadFromFile();
            for (int i = 0; i < playerName.Length; i++)
                EnterPlayerName(out playerName[i], i+1);
            
            while (true)
            {
                _view.DisplayMessage(_model.GetMessage("EnterWordPrompt"));
                primaryWord = _view.GetInput(out isCommand);
                if (isCommand)
                {
                    ProcessCommand(primaryWord);
                }
                else if (!_model.IsValidPrimaryWord(primaryWord))
                {
                    _view.DisplayMessage(_model.GetMessage("TryAgainPrompt"));
                }
                else break;
            }

            _model.SetAvailableLetters(primaryWord);
            _view.DisplayMessage(_model.GetMessage("PrimaryWordSuccess"), primaryWord);
            PlayGame();
        }

        private void PlayGame()
        {
            int currentPlayer = 0;
            int count = 0;

            while (true)
            {
                _view.DisplayMessage(_model.GetMessage("PlayerEnter"), playerName[currentPlayer]);
                (bool timeOut, string word) = _view.GetTimedInput(out isCommand);
                if (isCommand)
                {
                    ProcessCommand(word);
                }
                else
                {
                    if (timeOut)
                    {
                        _view.DisplayMessage(_model.GetMessage("TimeOut"));
                        return;
                    }

                    (bool isValid, string info) = _model.IsValidWord(word);

                    if (isValid)
                    {
                        _view.DisplayMessage(_model.GetMessage("PlayerEntered"), playerName[currentPlayer], word);
                        currentPlayer = currentPlayer == 0 ? 1 : 0;
                        count++;
                    }
                    else
                    {
                        _view.DisplayMessage(_model.GetMessage("PlayerLoose"), playerName[currentPlayer], word, info);
                        int winner = currentPlayer == 0 ? 1 : 0;
                        _model.UpdatePlayer(playerName[winner], count/2);
                        break;
                    }
                }
            }
        }
    }
}