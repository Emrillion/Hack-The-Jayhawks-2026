using Godot;
using System;

public partial class MainGameLoop : Node2D
{
	private PackedScene _foxScene = GD.Load<PackedScene>("res://scenes/fox.tscn");
	private PackedScene _jayhawkScene = GD.Load<PackedScene>("res://scenes/jayhawk.tscn");
	private PackedScene _jayhawkTwoScene = GD.Load<PackedScene>("res://scenes/jayhawktwo.tscn");
	private PackedScene _jayhawkThreeScene = GD.Load<PackedScene>("res://scenes/jayhawkthree.tscn");
	private PackedScene _jayhawkFourScene = GD.Load<PackedScene>("res://scenes/jayhawkfour.tscn");

	private Path2D _path;
	private float _spawnTimer = 0f;
	private float _spawnInterval = 2f;
	private uint heartsLeft = 10;
	private uint selectedTroop;
	private JayHawkTwo _selectedJayhawk;
	private uint _coins = 50;
	private WaveSpawner waveSpawnInstance;
	private Label coinText;
	// Question system
	private AcceptDialog _dialog;
	private LineEdit _answerInput;
	private Question[] _questions = new Question[]
	{
		new Question("What is 2+2?", "3", "4", "5", "6", 2),
		new Question("What color is the sky?", "Green", "Red", "Blue", "Yellow", 3),
		new Question("What is the capital of France?", "London", "Berlin", "Madrid", "Paris", 4),
	};
	private Question _currentQuestion;
	private Vector2 _pendingPlacePosition;
	private bool _waitingForAnswer = false;
private TextureRect _gameOverScreen;

	public override void _Ready()
	{
		var music = GetNode<AudioStreamPlayer2D>("MusicLoop");
		music.Play();
		coinText = GetNode<Label>("CoinLabel");
		waveSpawnInstance = GetNode<WaveSpawner>("WaveSpawner"); // adjust path as needed
		waveSpawnInstance.EnemyKilled += OnFoxKilled;
		waveSpawnInstance.EnemyReachedEnd += OnFoxReachedEnd;

		_gameOverScreen = GetNode<TextureRect>("UI/GameOver");
		_gameOverScreen.Visible = false;
		// Troop selection buttons
		GetNode<Button>("CanvasLayer/Troop0").Pressed += () => selectedTroop = 1;
		GetNode<Button>("CanvasLayer/Troop1").Pressed += () => selectedTroop = 2;
		GetNode<Button>("CanvasLayer/Troop2").Pressed += () => selectedTroop = 3;
		GetNode<Button>("CanvasLayer/Troop3").Pressed += () => selectedTroop = 4;
		GetNode<Button>("CanvasLayer/Troop4").Pressed += () => selectedTroop = 5;

		// Build dialog in code
		_dialog = new AcceptDialog();
		AddChild(_dialog);

		_path = GetNode<Path2D>("GamePath");
		GD.Print("path found: ", _path.Name);
	}

	private void AskQuestion(Vector2 position)
	{
		var random = new Random();
		_currentQuestion = _questions[random.Next(_questions.Length)];
		_pendingPlacePosition = position;
		_waitingForAnswer = true;

		// Clear old children
		foreach (var child in _dialog.GetChildren())
			child.QueueFree();

		_dialog.Title = "Answer to place your troop!";
		_dialog.DialogText = $"{_currentQuestion.Text}\n\n" +
							 $"A: {_currentQuestion.A}\n" +
							 $"B: {_currentQuestion.B}\n" +
							 $"C: {_currentQuestion.C}\n" +
							 $"D: {_currentQuestion.D}";

		_answerInput = new LineEdit();
		_answerInput.MaxLength = 1;
		_dialog.AddChild(_answerInput);

		// Always unsubscribe before subscribing to prevent stacking
		_dialog.Confirmed -= SubmitAnswer;
		_dialog.Canceled -= OnDialogCanceled;

		_dialog.Confirmed += SubmitAnswer;
		_dialog.Canceled += OnDialogCanceled;
		_dialog.PopupCentered();
	}

	private void OnDialogCanceled()
	{
		_dialog.Confirmed -= SubmitAnswer;
		_dialog.Canceled -= OnDialogCanceled;
		_waitingForAnswer = false;
		GD.Print("dialog cancelled, troop not placed");
	}

	private void SubmitAnswer()
	{
		_dialog.Confirmed -= SubmitAnswer;
		_dialog.Canceled -= OnDialogCanceled;

		string input = _answerInput.Text.Trim().ToUpper();

		int answer = input switch
		{
			"A" => 1,
			"B" => 2,
			"C" => 3,
			"D" => 4,
			_ => -1
		};

		if (answer == -1)
		{
			GD.Print("invalid input, troop not placed");
			_waitingForAnswer = false;
		}
		else
		{
			OnAnswer(answer);
		}
	}

	private void OnAnswer(int answer)
	{
		if (answer == _currentQuestion.CorrectAnswer)
		{
			GD.Print("correct!");
			PlaceTroop(_pendingPlacePosition);
		}
		else
		{
			GD.Print("wrong!");
		}
		_waitingForAnswer = false;
	}

	public override void _Input(InputEvent @event)
{
	if (@event is InputEventMouseButton mouse &&
		mouse.ButtonIndex == MouseButton.Left &&
		mouse.Pressed &&
		!_waitingForAnswer)
	{
		if (GetViewport().IsInputHandled())
			return;

		// Block if mouse is over any Control node (buttons, UI, etc.)
		if (IsMouseOverUI())
			return;

		var mousePos = GetGlobalMousePosition();
		var clicked = GetJayhawkAtPosition(mousePos);
		if (clicked != null)
		{
			_selectedJayhawk = clicked;
			GD.Print("selected jayhawk");
		}
		else if (selectedTroop != 0)
		{
			AskQuestion(mousePos);
		}
	}
}

private bool IsMouseOverUI()
{
	var mousePos = GetViewport().GetMousePosition();
	foreach (var node in GetNode("CanvasLayer").GetChildren())
	{
		if (node is Control control && control.Visible)
		{
			if (control.GetGlobalRect().HasPoint(mousePos))
				return true;
		}
	}
	return false;
}
	private JayHawkTwo GetJayhawkAtPosition(Vector2 pos)
	{
		foreach (var child in GetChildren())
		{
			if (child is JayHawkTwo hawk)
			{
				if (hawk.GlobalPosition.DistanceTo(pos) < 50f)
					return hawk;
			}
		}
		return null;
	}

	private void OnUpgradePressed()
	{
		if (_selectedJayhawk != null && IsInstanceValid(_selectedJayhawk))
			_selectedJayhawk.Upgrade();
	}
private void OnFoxKilled()
{
	_coins += 10;
	coinText.Text = "Coins: " + _coins;
}
	private void PlaceTroop(Vector2 position)
	{
		if (selectedTroop == 0)
		{
			GD.Print("no troop selected");
			return;
		}

		switch (selectedTroop)
		{
			case 1:
				if (_coins-5 < 0){break;}
				_coins -= 5;
				var jayhawk = _jayhawkScene.Instantiate<JayHawkBasic>();
				jayhawk.Position = position;
				AddChild(jayhawk);
				break;
			case 2:
				if (_coins-25 < 0){break;}
				_coins -= 25;
				var jayhawkTwo = _jayhawkTwoScene.Instantiate<JayHawkTwo>();
				jayhawkTwo.Position = position;
				AddChild(jayhawkTwo);
				break;
			case 3:
				if (_coins-30 < 0){break;}
				_coins -= 30;
				var jayhawkThree = _jayhawkThreeScene.Instantiate<JayHawkThree>();
				jayhawkThree.Position = position;
				AddChild(jayhawkThree);
				break;
			case 4:
				if (_coins-50 < 0){break;}
				_coins -= 50;
				var jayhawkFour = _jayhawkFourScene.Instantiate<JayHawkFour>();
				jayhawkFour.Position = position;
				AddChild(jayhawkFour);
				break;
}
			coinText.Text = "Coins: " + _coins;

	}

// update OnFoxReachedEnd
private void OnFoxReachedEnd()
{
	heartsLeft -= 1;
	GD.Print("health remaining: ", heartsLeft);
	if (heartsLeft == 0)
	{
		_gameOverScreen.Visible = true;
		GetTree().Paused = true;
	}
}

	public override void _Process(double delta)
	{
		GD.Print(heartsLeft);
		_spawnTimer += (float)delta;
		if (_spawnTimer >= _spawnInterval)
		{
		}
	}

	private void SpawnFox()
	{
		var fox = _foxScene.Instantiate<Enemy>();
		fox.Scale = new Vector2(0.5f, 0.5f);
		fox.ReachedEnd += OnFoxReachedEnd;
		fox.Loop = false;
		_path.AddChild(fox);
		GD.Print("fox spawned");
	}
}
