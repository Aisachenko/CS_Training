using System.Globalization;
using System.Resources;

namespace Task1
{

    public class Model
    {
        public string SelectedLanguage { get; set; } = "ru";
        public Dictionary<char, int> AvailableLetters { get; private set; }
        private HashSet<string> usedWords = new HashSet<string>();
        private ResourceManager resourceManager;
    
        public Model()
        {
            resourceManager = new ResourceManager("Task1.Resources.Messages", typeof(Model).Assembly);
        }
    
        public string GetMessage(string messageKey)
        {
            CultureInfo culture = SelectedLanguage == "en" ? CultureInfo.CreateSpecificCulture("en") : CultureInfo.InvariantCulture;
            return resourceManager.GetString(messageKey, culture) ?? messageKey;
        }

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
                return (false, GetMessage("AlreadyExistsMessage"));
            if (!word.All(char.IsLetter))
                return (false, GetMessage("AllLettersMessage"));

            var letterCount = CreateDictionary(word);
            foreach (var letter in letterCount.Keys)
            {
                if (!AvailableLetters.ContainsKey(letter) || letterCount[letter] > AvailableLetters[letter])
                {
                    return (false, GetMessage("InvalidCharacterCountMessage"));
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
    }
}