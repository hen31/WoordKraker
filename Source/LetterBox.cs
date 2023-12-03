using Godot;
using System;

public enum LetterState
{
	Unknown,
	InWordNotRightPlace,
	InRightPlace
};

public partial class LetterBox : PanelContainer
{
	private StyleBox _currentStyleBox = null;
	public LetterState CurrentState { get; set; } = LetterState.Unknown;
	[Export] public Label LetterLabel { get; set; }

	[Export] public PanelContainer FocusContainer { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _GuiInput(InputEvent iEvent)
	{
		if (iEvent is InputEventMouse mouseEvent)
		{
			if (iEvent.IsPressed())
			{
				Node parent = GetParent();
				while (!(parent is GameScene))
				{
					parent = parent.GetParent();
				}

				var gameScene = (GameScene)parent;
				if (gameScene.CurrentAttempt.LetterBoxes.IndexOf(this) >= 0)
				{
					gameScene.SetSelectedLetterBox(this);
				}
			}
		}
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetLetterStyle(StyleBox styleBox)
	{
		_currentStyleBox = styleBox;
		AddThemeStyleboxOverride("panel", styleBox);
	}

	public bool HasStyleBox(StyleBox styleBox)
	{
		return _currentStyleBox == styleBox;
	}

	public char GetCheckCharacter()
	{
		if (LetterLabel.Text == "IJ")
		{
			return ';';
		}

		if (LetterLabel.Text == "")
		{
			return '0';
		}
		return LetterLabel.Text[0];
	}
}
