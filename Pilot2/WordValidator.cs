namespace Task1;

public class WordValidator
{

    public Dictionary<char, int> AvailableLetters { get; private set; }
    public HashSet<string> usedWords = new HashSet<string>();
    private Localization _localization = new Localization();

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
            return (false, _localization.GetMessage("AlreadyExistsMessage"));
        if (!word.All(char.IsLetter))
            return (false, _localization.GetMessage("AllLettersMessage"));

        var letterCount = CreateDictionary(word);
        foreach (var letter in letterCount.Keys)
        {
            if (!AvailableLetters.ContainsKey(letter) || letterCount[letter] > AvailableLetters[letter] ||
                letterCount.Count == 0)
            {
                return (false, _localization.GetMessage("InvalidCharacterCountMessage"));
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

    public HashSet<string> UsedWords()
    {
        return usedWords;
    }
}