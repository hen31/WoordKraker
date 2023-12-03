using Godot;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Godot.Collections;
using WoordKraker;
using Environment = System.Environment;

public partial class GameScene : Control
{
    private int _wordsHad, _wordsRight = 0;

    private string _currentWord = "pr;st";
    private double _noWordTimer;
    [Export] public Array<AttemptUI> Attempts { get; set; }

    [Export] public VirtualKeyBoard KeyBoard { get; set; }
    [Export] public Control NoWordScreen { get; set; }
    [Export] public Control EndScreen { get; set; }
    [Export] public Label EndLabel { get; set; }
    [Export] public Label WordsLabel { get; set; }
    [Export] public Label HighScoreLabel { get; set; }
    [Export] public Label TimeLabel { get; set; }
    [Export] public Button EndButton { get; set; }
    [Export] public Button EnterButton { get; set; }
    [Export] public Button BackButton { get; set; }

    public AttemptUI CurrentAttempt { get; set; }
    public LetterBox CurrentSelectedLetterBox { get; set; }

    [Export] public StyleBox SelectionStyleBox { get; set; }
    [Export] public StyleBox RightPlaceStyleBox { get; set; }
    [Export] public StyleBox RightWrongPlaceStyleBox { get; set; }

    [Export] public StyleBox NormalLetterStyleBox { get; set; }
    [Export] public StyleBox HadLetterStyleBox { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ResetGame();
        EnterButton.Pressed += EnterPressed;
        EndButton.Pressed += EndButtonOnPressed;
        BackButton.Pressed += BackPressed;
        EndButton.Text = "Volgende woord";
    }

    private void BackPressed()
    {
        GetTree().ChangeSceneToFile("res://Menu.tscn");
    }

    private bool _ended = false;

    private void EndButtonOnPressed()
    {
        if (_ended && (WordSelector.Instance.GetGameMode() == GameMode.SuddenDeath ||
                       WordSelector.Instance.GetGameMode() == GameMode.TimeAttack))
        {
            GetTree().ChangeSceneToFile("res://Menu.tscn");
        }

        ResetGame();
        EndScreen.Visible = false;
    }

    private void ResetGame()
    {
        HighScoreService.Instance.SetHighScoreIfHigher(WordSelector.Instance.GetGameMode(), _wordsRight);
        HighScoreLabel.Text = "Hoogste score: " +
                              HighScoreService.Instance.GetHighScoreForMode(WordSelector.Instance.GetGameMode());
        _paused = false;
        _wordsHad++;
        KeyBoard.Reset();
        foreach (var attempt in Attempts)
        {
            attempt.Reset(NormalLetterStyleBox);
        }

        SetCurrentAttempt(Attempts[0]);
        _currentWord = WordSelector.Instance.GetRandomWord();
        Debug.WriteLine(_currentWord);
        WordsLabel.Text = $"{_wordsRight}/{_wordsHad}";
    }

    private double _timer;
    private bool _paused = false;

    public override void _Process(double delta)
    {
        if (!_paused)
        {
            double totalTime = 180.0;
            _timer += delta;
            if (WordSelector.Instance.GetGameMode() == GameMode.TimeAttack)
            {
                totalTime -= _timer;

                var seconds = (int)totalTime % 60;
                var minutes = Mathf.Floor(totalTime / 60.0);
                if (totalTime <= 0.0)
                {
                    NoWordScreen.Visible = false;
                    EndButton.Text = "Terug";
                    EndLabel.Text = $"Helaas, het woord was '{_currentWord.ToUpper().Replace(";", "IJ")}'";
                    EndLabel.Text += Environment.NewLine;

                    EndLabel.Text += $"Eindscore was {_wordsRight} van de {_wordsHad}";
                    _ended = true;

                    EndScreen.Visible = true;
                    EndButton.GrabFocus();
                    return;
                }

                TimeLabel.Text = $"{minutes:00}:{seconds:00}";
            }
            else
            {
                var seconds = (int)_timer % 60;
                var minutes = Mathf.Floor(_timer / 60.0);
                TimeLabel.Text = $"{minutes:00}:{seconds:00}";
            }
        }

        if (NoWordScreen.Visible)
        {
            _noWordTimer += delta;
            if (_noWordTimer > 2f)
            {
                NoWordScreen.Visible = false;
            }
        }
    }

    public void SetCurrentAttempt(AttemptUI attempt)
    {
        CurrentAttempt = attempt;
        SetSelectedLetterBox(attempt.LetterBoxes[0]);
    }

    public void SetSelectedLetterBox(LetterBox letterBox)
    {
        CurrentSelectedLetterBox?.FocusContainer.AddThemeStyleboxOverride("panel", new StyleBoxEmpty());
        letterBox.FocusContainer.AddThemeStyleboxOverride("panel", SelectionStyleBox);
        CurrentSelectedLetterBox = letterBox;
    }

    public override void _UnhandledKeyInput(InputEvent inputEvent)
    {
        if (inputEvent is InputEventKey eventKey && eventKey.IsReleased())
        {
            if (eventKey.Keycode == Key.Enter && !EndScreen.Visible)
            {
                EnterPressed();
                return;
            }

            if (eventKey.Keycode == Key.Backspace)
            {
                VirtualKeyPressed("del");
                return;
            }

            var text = eventKey.AsTextKeyLabel();
            if (text.Length == 1 && Regex.IsMatch(text, "[a-z]", RegexOptions.IgnoreCase))
            {
                VirtualKeyPressed(text);
            }
        }
    }

    private void VirtualKeyPressed(string character)
    {
        var currentLetterIndex = CurrentAttempt.LetterBoxes.IndexOf(CurrentSelectedLetterBox);

        if (character == "del")
        {
            if (CurrentSelectedLetterBox.LetterLabel.Text == "")
            {
                if (currentLetterIndex > 0)
                {
                    SetSelectedLetterBox(CurrentAttempt.LetterBoxes[currentLetterIndex - 1]);
                    CurrentSelectedLetterBox.LetterLabel.Text = "";
                    return;
                }
            }

            CurrentSelectedLetterBox.LetterLabel.Text = "";
            return;
        }

        if (character != "IJ" && currentLetterIndex > 0)
        {
            string previousAndThis = CurrentAttempt.LetterBoxes[currentLetterIndex - 1].LetterLabel.Text + character;
            if (previousAndThis == "IJ")
            {
                CurrentAttempt.LetterBoxes[currentLetterIndex - 1].LetterLabel.Text = previousAndThis;
                return;
            }
        }

        CurrentSelectedLetterBox.LetterLabel.Text = character;


        if (currentLetterIndex < 4)
        {
            SetSelectedLetterBox(CurrentAttempt.LetterBoxes[currentLetterIndex + 1]);
        }
    }

    private void EnterPressed()
    {
        NoWordScreen.Visible = false;
        if (CurrentAttempt.LetterBoxes.Any(b => b.LetterLabel.Text == ""))
        {
            Debug.WriteLine("Empty box");
            return;
        }


        string currentWord = string.Join("", CurrentAttempt.LetterBoxes.Select(b => b.LetterLabel.Text).ToArray())
            .Replace("IJ", ";");

        if (!WordSelector.Instance.IsRealWord(currentWord))
        {
            _noWordTimer = 0.0;
            NoWordScreen.Visible = true;
            return;
        }

        if (currentWord.Equals(_currentWord, StringComparison.OrdinalIgnoreCase))
        {
            foreach (var letterBox in CurrentAttempt.LetterBoxes)
            {
                letterBox.SetLetterStyle(RightPlaceStyleBox);
            }


            _paused = true;
            EndLabel.Text = $"U heeft het woord geraden het was '{_currentWord.ToUpper().Replace(";", "IJ")}'";
            EndScreen.Visible = true;
            EndButton.GrabFocus();
            _wordsRight++;


            return;
        }

        foreach (var character in _currentWord.ToUpper().Distinct())
        {
            var amount = _currentWord.ToUpper().Count(b => b == character);
            var matchingLetterBoxes = CurrentAttempt.LetterBoxes.Where(b =>
                b.HasStyleBox(NormalLetterStyleBox) &&
                character == b.GetCheckCharacter() &&
                character == _currentWord.ToUpper()[CurrentAttempt.LetterBoxes.IndexOf(b)]);
            foreach (var matchingLetterBox in matchingLetterBoxes)
            {
                matchingLetterBox.SetLetterStyle(RightPlaceStyleBox);
                amount--;
            }

            if (amount > 0)
            {
                var sameLetterOtherBox = CurrentAttempt.LetterBoxes.Where(b =>
                        b.HasStyleBox(NormalLetterStyleBox) &&
                        character == b.GetCheckCharacter() &&
                        character != _currentWord.ToUpper()[CurrentAttempt.LetterBoxes.IndexOf(b)])
                    .OrderBy(b => CurrentAttempt.LetterBoxes.IndexOf(b));
                foreach (var sameLetterBox in sameLetterOtherBox)
                {
                    sameLetterBox.SetLetterStyle(RightWrongPlaceStyleBox);
                    amount--;
                    if (amount == 0)
                    {
                        break;
                    }
                }
            }
        }

        var noStyle = CurrentAttempt.LetterBoxes.Where(b =>
            b.HasStyleBox(NormalLetterStyleBox));
        foreach (var noMatchLetterBox in noStyle)
        {
            noMatchLetterBox.SetLetterStyle(HadLetterStyleBox);
        }

        var attemptIndex = Attempts.IndexOf(CurrentAttempt);
        if (attemptIndex < 5)
        {
            SetCurrentAttempt(Attempts[attemptIndex + 1]);
            KeyBoard.UpdateState(this);
            return;
        }

        if (WordSelector.Instance.GetGameMode() == GameMode.Endless ||
            WordSelector.Instance.GetGameMode() == GameMode.TimeAttack)
        {
            _paused = true;
            EndLabel.Text = $"Helaas, het woord was '{_currentWord.ToUpper().Replace(";", "IJ")}'";
            EndScreen.Visible = true;
            EndButton.GrabFocus();
        }
        else if (WordSelector.Instance.GetGameMode() == GameMode.SuddenDeath)
        {
            EndButton.Text = "Terug";
            EndLabel.Text = $"Helaas, het woord was '{_currentWord.ToUpper().Replace(";", "IJ")}'";
            EndLabel.Text += Environment.NewLine;
            var seconds = (int)_timer % 60;
            var minutes = Mathf.Floor(_timer / 60.0);
            EndLabel.Text += $"Eindscore was {_wordsRight} in {minutes:00}:{seconds:00}";
            _ended = true;

            EndScreen.Visible = true;
            EndButton.GrabFocus();
        }
    }
}