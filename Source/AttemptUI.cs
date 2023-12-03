using System;
using Godot;
using Godot.Collections;

public partial class AttemptUI : HBoxContainer
{
	[Export] public Array<LetterBox> LetterBoxes { get; set; } = new Array<LetterBox>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Reset(StyleBox letterStyle)
	{
		foreach (var letterBox in LetterBoxes)
		{
			letterBox.LetterLabel.Text = "";
			letterBox.SetLetterStyle(letterStyle);
		}
	}
}
