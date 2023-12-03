using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class VirtualKeyBoard : Control
{
	[Signal]
	public delegate void VirtualKeyPressedEventHandler(string character);

	private Godot.Collections.Dictionary<char, Button> _buttons = new Godot.Collections.Dictionary<char, Button>();

	public List<char> buttonLayout = new List<char>()
	{
		'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p',
		'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', ' ',
		' ', 'z', 'x', 'c', 'v', 'b', 'n', 'm', '7', '8'
	};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		char[] wideKeys = new[]
		{
			'q', 'p', 'a', 'l', 'z', '8'
		};
		var buttonSize = new Vector2(50, 40);
		int i = 0;
		HBoxContainer currentRow = new HBoxContainer();
		currentRow.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		AddChild(currentRow);
		foreach (var c in buttonLayout)
		{
			if (i == 10)
			{
				i = 0;
				currentRow = new HBoxContainer();
				currentRow.SizeFlagsHorizontal = SizeFlags.ExpandFill;
				AddChild(currentRow);
			}

			if (c == ' ')
			{
				i++;
				continue;
			}

			if (c == '7')
			{
				Button specialCase = new Button();
				specialCase.CustomMinimumSize = buttonSize;

				_buttons.Add(c, specialCase);
				specialCase.Pressed += () => EmitSignalPressed("ij".ToUpper());
				specialCase.Text = "ij".ToUpper();
				currentRow.AddChild(specialCase);
				i++;
				continue;
			}

			if (c == '8')
			{
				Button deleteBtn = new Button();
				deleteBtn.CustomMinimumSize = buttonSize;
				_buttons.Add(c, deleteBtn);

				deleteBtn.Text = "\u232b";
				deleteBtn.Pressed += () => EmitSignalPressed("del");
				currentRow.AddChild(deleteBtn);
				deleteBtn.SizeFlagsHorizontal = SizeFlags.ExpandFill;
				i++;
				continue;
			}

			Button b = new Button();
			b.CustomMinimumSize = buttonSize;
			b.Pressed += () => EmitSignalPressed(new string(c, 1).ToUpper());

			if (wideKeys.Contains(c))
			{
				b.SizeFlagsHorizontal = SizeFlags.ExpandFill;
			}

			_buttons.Add(c, b);

			b.Text = new string(c, 1).ToUpper();
			currentRow.AddChild(b);
			i++;
		}
	}


	private void EmitSignalPressed(string key)
	{
		EmitSignal(SignalName.VirtualKeyPressed, key);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Reset()
	{
		foreach (var character in _buttons.Keys)
		{
			_buttons[character].RemoveThemeColorOverride("font_color");
			_buttons[character].RemoveThemeColorOverride("font_focus_color");

		}
	}

	public void UpdateState(GameScene gameScene)
	{
		foreach (var character in _buttons.Keys)
		{
			var checkChar = char.ToUpper(character);
			var letterBoxes = gameScene.Attempts.SelectMany(b => b.LetterBoxes)
				.Where(b => b.GetCheckCharacter() == checkChar).ToList();
			if (letterBoxes.Any())
			{
				if (letterBoxes.Any(lb => lb.HasStyleBox(gameScene.RightPlaceStyleBox)))
				{
					_buttons[character].AddThemeColorOverride("font_color", Colors.Green);
					_buttons[character].AddThemeColorOverride("font_focus_color", Colors.Green);
				}
				else if (letterBoxes.Any(lb => lb.HasStyleBox(gameScene.RightWrongPlaceStyleBox)))
				{
					_buttons[character].AddThemeColorOverride("font_color", Colors.Yellow);
					_buttons[character].AddThemeColorOverride("font_focus_color", Colors.Yellow);
				}
				else
				{
					_buttons[character].AddThemeColorOverride("font_color", Colors.Blue);
					_buttons[character].AddThemeColorOverride("font_focus_color", Colors.Blue);
				}
			}
		}
	}
}
