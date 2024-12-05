using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static bool IsValidPrimaryWord(string primaryWord)
    {
        if (string.IsNullOrWhiteSpace(primaryWord) || primaryWord.Length < 8 || primaryWord.Length > 30 || !primaryWord.All(char.IsLetter))
        {
            Console.WriteLine("Ошибка: длина слова должна быть от 8 до 30 символов и все символы - буквы.");
            return false;
        }
        return true;
    }
    
    static (bool isValid, string info) IsValidWord(string word, Dictionary<char, int> availableLetters, HashSet<string> usedWords)
    {
        if (usedWords.Contains(word))
            return (false, "Уже есть такое слово.");
        if ( !word.All(char.IsLetter))
            return (false, "Все символы должны быть буквы.");
        var letterCount = new Dictionary<char, int>();
        letterCount = MakeDictionary(word);
        foreach (var letter in letterCount.Keys)
        {
            if (!availableLetters.ContainsKey(letter) || letterCount[letter] > availableLetters[letter])
            {
                return (false, "Содержит неверное количество повторяющихся символов или новые символы.");
            }
        }
        return (true, "OK");
    }
    
    static Dictionary<char, int> MakeDictionary(string word)
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

    static void Game(Dictionary<char, int> availableLetters)
    {
        var usedWords = new HashSet<string>();
        int currentPlayer = 1;

        while (true)
        {
            Console.WriteLine($"Игрок {currentPlayer}, введите слово:");
            string word = Console.ReadLine().ToLower();
            (bool isValid, string info) = IsValidWord(word, availableLetters, usedWords);
            if (isValid)
            {
                usedWords.Add(word);
                Console.WriteLine($"Игрок {currentPlayer} ввел слово: {word}");
                currentPlayer = currentPlayer == 1 ? 2 : 1;
            }
            else
            {
                Console.WriteLine($"Игрок {currentPlayer} проиграл! Невозможно использовать слово: {word}" +
                                  $"\nПричина: {info}");
                break;
            }
        }
        
    }
    
    static void Main(string[] args)
    {
        string primaryWord;
        var availableLetters = new Dictionary<char, int>();
        
        Console.WriteLine("Введите слово длиной от 8 до 30 символов:");
        
        while (!IsValidPrimaryWord(primaryWord = Console.ReadLine()))
        {
            Console.WriteLine("Попробуйте снова: ");
        }
        Console.WriteLine("Успешно! Первичное слово: " + primaryWord);
        
        availableLetters = MakeDictionary(primaryWord);

        Game(availableLetters);
    }
    
}