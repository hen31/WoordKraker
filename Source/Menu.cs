using Godot;
using System;
using WoordKraker;
using Environment = Godot.Environment;

public enum GameMode
{
	Endless,
	TimeAttack,
	SuddenDeath
}

public partial class Menu : Control
{
	[Export] public PanelContainer FirstPanel { get; set; }
	[Export] public PanelContainer SecondPanel { get; set; }
	[Export] public PanelContainer ThirdPanel { get; set; }
	[Export] public PanelContainer InfoPanel { get; set; }
	[Export] public Label InfoLabel { get; set; }

	[Export] public Button PlayEndlessButton { get; set; }
	[Export] public Button PlayTimeAttackButton { get; set; }
	[Export] public Button PlaySuddenDeathButton { get; set; }
	
	[Export] public Button DailyButton { get; set; }
	[Export] public Button RandomButton { get; set; }
	[Export] public Button SpecificButton { get; set; }
	[Export] public Button SpecificPlayButton { get; set; }
	
	[Export] public Button Back2Button { get; set; }
	[Export] public Button Back3Button { get; set; }
	[Export] public Button BackInfoButton { get; set; }
	
	[Export] public Button InfoEndlessButton { get; set; }
	[Export] public Button InfoTimeAttackButton { get; set; }
	[Export] public Button InfoSuddenDeathAttackButton { get; set; }
	
	[Export] public SpinBox SpinBox { get; set; }

	private GameMode _selectedGameMode;
	public int Seed { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Seed = int.Parse(DateTime.Now.ToString("ddMMyy"));
		SpinBox.Value = Seed;
		PlayEndlessButton.Pressed += ()=> PlayButtonPressed(GameMode.Endless);
		PlayTimeAttackButton.Pressed += ()=> PlayButtonPressed(GameMode.TimeAttack);
		PlaySuddenDeathButton.Pressed += ()=> PlayButtonPressed(GameMode.SuddenDeath);
		DailyButton.Pressed += DailyPressed;
		RandomButton.Pressed += RandomPressed;
		SpecificButton.Pressed += SpecificPressed;
		SpecificPlayButton.Pressed += SpecificPlayPressed;
		Back2Button.Pressed += () =>
		{
			SecondPanel.Visible = false;
			FirstPanel.Visible = true;
		};
		Back3Button.Pressed += () =>
		{
			SecondPanel.Visible = true;
			ThirdPanel.Visible = false;
		};
		BackInfoButton.Pressed += () =>
		{
			InfoPanel.Visible = false;
			FirstPanel.Visible = true;
		};

		InfoEndlessButton.Pressed += () =>
		{
			InfoLabel.Text =
				"Eindeloos spel\nu speelt woord na woord. Zonder limiet op hoelang of hoe vaak. Er is geen einde, het spel stopt pas als u op terug drukt.";
			InfoPanel.Visible = true;
			FirstPanel.Visible = false;
		};
		
		InfoTimeAttackButton.Pressed += () =>
		{
			InfoLabel.Text =
				"Time attack\nRaad zoveel mogelijk woorden binnen 3 minuten. Wilt u het vergelijken met iemand anders, doe dan de dagelijkse uitdaging. Iedereen die het die dag speelt krijgt dezelfde woorden.";
			InfoPanel.Visible = true;
			FirstPanel.Visible = false;
		};
		
		InfoSuddenDeathAttackButton.Pressed += () =>
		{
			InfoLabel.Text =
				"Sudden death\nHoeveel woorden kunt u raden zonder een fout te maken? Raad u een woord niet binnen 6 pogingen, dan is het gameover.";
			InfoPanel.Visible = true;
			FirstPanel.Visible = false;
		};
	}

	private void SpecificPlayPressed()
	{
		WordSelector.Instance.SetRandom(Seed);
		WordSelector.Instance.ChangeGameMode(_selectedGameMode);
		GetTree().ChangeSceneToFile("res://MainGame.tscn");
	}

	private void RandomPressed()
	{
		WordSelector.Instance.SetRandom(new Random().Next());
		WordSelector.Instance.ChangeGameMode(_selectedGameMode);
		GetTree().ChangeSceneToFile("res://MainGame.tscn");
	}

	private void SpecificPressed()
	{
		SecondPanel.Visible = false;
		ThirdPanel.Visible = true;
	}

	private void DailyPressed()
	{
		var seed = int.Parse(DateTime.Now.ToString("ddMMyy"));
		var rand = new Random(seed);
		for (int i = 0; i < (int)_selectedGameMode; i++)
		{
			seed = rand.Next();
		}
		WordSelector.Instance.SetRandom(seed);
		WordSelector.Instance.ChangeGameMode(_selectedGameMode);
		GetTree().ChangeSceneToFile("res://MainGame.tscn");
	}

	private void PlayButtonPressed(GameMode mode)
	{
		_selectedGameMode = mode;
		FirstPanel.Visible = false;
		SecondPanel.Visible = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
