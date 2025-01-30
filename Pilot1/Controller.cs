namespace Task1
{
    public class Controller
    {
        private Model _model = new Model();
        private View _view = new View();

        private void SetLanguage()
        {
            _view.DisplayMessage("Choose language / Выберите язык:");
            _view.DisplayMessage("1. English");
            _view.DisplayMessage("2. Русский");

            var choice = _view.GetInput();
            _model.SelectedLanguage = choice == "1" ? "en" : "ru";
        }

        public void StartGame()
        {
            SetLanguage();
            string primaryWord;

            _view.DisplayMessage(_model.GetMessage("EnterWordPrompt"));
            while (!_model.IsValidPrimaryWord(primaryWord = _view.GetInput()))
            {
                _view.DisplayMessage(_model.GetMessage("TryAgainPrompt"));
            }

            _model.SetAvailableLetters(primaryWord);
            _view.DisplayMessage(_model.GetMessage("PrimaryWordSuccess"), primaryWord);
            PlayGame();
            return;
        }

        private void PlayGame()
        {
            int currentPlayer = 1;

            while (true)
            {
                _view.DisplayMessage(_model.GetMessage("PlayerEnter"), currentPlayer);
                (bool timeOut, string word) = _view.GetTimedInput();
                if (timeOut)
                {
                    _view.DisplayMessage(_model.GetMessage("TimeOut"));
                    return; 
                }

                (bool isValid, string info) = _model.IsValidWord(word);

                if (isValid)
                {
                    _view.DisplayMessage(_model.GetMessage("PlayerEntered"), currentPlayer, word);
                    currentPlayer = currentPlayer == 1 ? 2 : 1;
                }
                else
                {
                    _view.DisplayMessage(_model.GetMessage("PlayerLoose"), currentPlayer, word, info);
                    break; 
                }
            }
        }
        
    }
}