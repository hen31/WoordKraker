using System;
using Godot;
using Environment = Godot.Environment;

namespace WoordKraker;

public class HighScoreService
{
	private string fileName = "user://high_score.txt";

	private int _endlessHigh = 0;
	private int _timeHigh = 0;
	private int _suddenHigh = 0;

	private static HighScoreService _instance;

	public static HighScoreService Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new HighScoreService();
			}

			return _instance;
		}
	}


	public HighScoreService()
	{
		//TODO:LOAD
		FileAccess file = FileAccess.Open(fileName, FileAccess.ModeFlags.Read);
		if (file!=null && file.GetError() == Error.Ok)
		{
			var lines = file.GetAsText().Split(System.Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
			_endlessHigh = int.Parse(lines[0]);
			_timeHigh = int.Parse(lines[1]);
			_suddenHigh = int.Parse(lines[2]);
			file.Close();
		}
	}

	public int GetHighScoreForMode(GameMode mode)
	{
		switch (mode)
		{
			case GameMode.Endless:
				return _endlessHigh;
			case GameMode.TimeAttack:
				return _timeHigh;
			case GameMode.SuddenDeath:
				return _suddenHigh;
			default:
				return -1;
		}
	}

	public void SetHighScoreIfHigher(GameMode mode, int score)
	{
		switch (mode)
		{
			case GameMode.Endless:
				if (_endlessHigh < score)
				{
					_endlessHigh = score;
					SaveScores();
				}
				break;
			case GameMode.TimeAttack:
				if (_timeHigh < score)
				{
					_timeHigh = score;
					SaveScores();
				}
				break;
			case GameMode.SuddenDeath:
				if (_suddenHigh < score)
				{
					_suddenHigh = score;
					SaveScores();
				}
				break;
		}
	}

	private void SaveScores()
	{
		FileAccess file = FileAccess.Open(fileName, FileAccess.ModeFlags.Write);
		string scores = _endlessHigh + System.Environment.NewLine + _timeHigh +
						System.Environment.NewLine + _suddenHigh;
		file.StoreString(scores);
		file.Close();
	}
}
