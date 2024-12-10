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
            _model.selectedLanguage = choice == "1" ? "en" : "ru";
        }

        public void StartGame()
        {
            SetLanguage();
            string primaryWord;

            _view.DisplayMessage(_model.GetMessage("Введите слово длиной от 8 до 30 символов:"));
            while (!_model.IsValidPrimaryWord(primaryWord = _view.GetInput()))
            {
                _view.DisplayMessage(_model.GetMessage("Попробуйте снова: "));
            }

            _model.SetAvailableLetters(primaryWord);
            _view.DisplayMessage(_model.GetMessage("Успешно! Первичное слово: "), primaryWord);
            PlayGame();
        }

        private void PlayGame()
        {
            int currentPlayer = 1;

            while (true)
            {
                _view.DisplayMessage(_model.GetMessage("Игрок {0}, введите слово:"), currentPlayer);
                string word = _view.GetInput();
                (bool isValid, string info) = _model.IsValidWord(word);

                if (isValid)
                {
                    _view.DisplayMessage(_model.GetMessage("Игрок {0} ввел слово: {1}"), currentPlayer, word);
                    currentPlayer = currentPlayer == 1 ? 2 : 1;
                }
                else
                {
                    _view.DisplayMessage(
                        _model.GetMessage("Игрок {0} проиграл! Невозможно использовать слово: {1}\nПричина: {2}"),
                        currentPlayer, word, info);
                    break;
                }
            }
        }
    }
}