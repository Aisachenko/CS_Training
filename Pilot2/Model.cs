using System.Globalization;
using System.Resources;
using Newtonsoft.Json;

namespace Task1
{

    public class Model
    {
        public string SelectedLanguage { get; set; } = "ru";
        public Dictionary<char, int> AvailableLetters { get; private set; }
        private HashSet<string> usedWords = new HashSet<string>();
        private ResourceManager resourceManager;
        static string fileName = "/gameData.json";
        string filePath = Environment.CurrentDirectory + fileName;
        
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
                if (!AvailableLetters.ContainsKey(letter) || letterCount[letter] > AvailableLetters[letter] || letterCount.Count == 0)
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

        public HashSet<string> UsedWords()
        {
            return usedWords;
        }

        public void AddPlayer(string name, int score = 0)
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

    public class Player
    {
        public string Name;
        public int Score;   //максимальное количество слов в победной игре

        public Player(string name, int score = 0)
        {
            Name = name;
            Score = score;
        }
    }
}