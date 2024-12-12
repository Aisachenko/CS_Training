namespace Task1
{

    public class Model
    {
        public string SelectedLanguage { get; set; } = "ru";
        public Dictionary<char, int> AvailableLetters { get; private set; }
        private HashSet<string> usedWords = new HashSet<string>();

        public bool IsValidPrimaryWord(string primaryWord)
        {
            return !string.IsNullOrWhiteSpace(primaryWord) && 
                   primaryWord.Length >= 8 && 
                   primaryWord.Length <= 30 && 
                   primaryWord.All(char.IsLetter);
        }

        public (bool isValid, string info) IsValidWord(string word)
        {
            if (usedWords.Contains(word))
                return (false, GetMessage("Уже есть такое слово."));
            if (!word.All(char.IsLetter))
                return (false, GetMessage("Все символы должны быть буквы."));

            var letterCount = CreateDictionary(word);
            foreach (var letter in letterCount.Keys)
            {
                if (!AvailableLetters.ContainsKey(letter) || letterCount[letter] > AvailableLetters[letter])
                {
                    return (false, GetMessage("Содержит неверное количество повторяющихся символов или новые символы."));
                }
            }
            usedWords.Add(word);
            return (true, "OK");
        }

        public void SetAvailableLetters(string primaryWord)
        {
            AvailableLetters = CreateDictionary(primaryWord);
        }

        private Dictionary<char, int> CreateDictionary(string word)
        {
            var letters = new Dictionary<char, int>();
            foreach (var letter in word)
            {
                if (letters.ContainsKey(letter))
                    letters[letter]++;
                else
                    letters[letter] = 1;
            }
            return letters;
        }

        public string GetMessage(string message)
        {
            if (SelectedLanguage == "en")
            {
                switch (message)
                {
                    case "Введите слово длиной от 8 до 30 символов:":
                        return "Enter a word between 8 and 30 characters:";
                    case "Попробуйте снова: ":
                        return "Try again: ";
                    case "Успешно! Первичное слово: ":
                        return "Success! Primary word: ";
                    case "Ошибка: длина слова должна быть от 8 до 30 символов и все символы - буквы.":
                        return "Error: The word length must be between 8 and 30 characters and all characters must be letters.";
                    case "Уже есть такое слово.":
                        return "This word already exists.";
                    case "Все символы должны быть буквы.":
                        return "All characters must be letters.";
                    case "Содержит неверное количество повторяющихся символов или новые символы.":
                        return "Contains incorrect number of repeating characters or new characters.";
                    case "Игрок {0}, введите слово:":
                        return "Player {0}, enter a word:";
                    case "Игрок {0} ввел слово: {1}":
                        return "Player {0} entered the word: {1}";
                    case "Игрок {0} проиграл! Невозможно использовать слово: {1}\nПричина: {2}":
                        return "Player {0} lost! Unable to use the word: {1}\nReason: {2}";
                    default:
                        return "Unable to translate "+ message;
                }
            }
            else
            {
                return message;
            }
        }
    }
}