using System;
using System.IO;
using System.Linq;
using System.Reflection;
using FileAccess = Godot.FileAccess;

namespace WoordKraker;

public class WordSelector
{
    private Random _rand;
    private static WordSelector _instance = new WordSelector();
    public static WordSelector Instance => _instance;
    private string[] _wordsToCheck;
    private string[] _wordsToSelect;
    private GameMode _currentGameMode;

    public WordSelector()
    {
        FileAccess file = FileAccess.Open("res://wordlist.txt", FileAccess.ModeFlags.Read);

        string splitChar = "\r\n";

        string result = file.GetAsText();
        if (!result.Contains(splitChar))
        {
            splitChar = "\n";
        }
        _wordsToCheck = result.Split(splitChar, StringSplitOptions.RemoveEmptyEntries).Select(b => b.TrimEnd('1'))
            .ToArray();
        _wordsToSelect = result.Split(splitChar, StringSplitOptions.RemoveEmptyEntries).Where(b => b.Last() == '1')
            .Select(b => b.TrimEnd('1')).ToArray();
    }

    public void SetRandom(int seed)
    {
        _rand = new Random(seed);
    }

    public bool IsRealWord(string word)
    {
        return _wordsToCheck.Contains(word);
    }


    public string GetRandomWord()
    {
        return _wordsToSelect[_rand.Next(0, _wordsToSelect.Length - 1)];
    }

    public void ChangeGameMode(GameMode gameMode)
    {
        _currentGameMode = gameMode;
    }

    public GameMode GetGameMode()
    {
        return _currentGameMode;
    }
}