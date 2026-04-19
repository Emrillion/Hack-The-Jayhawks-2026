using Godot;

public partial class MainGameLoop : Node2D
{
	private PackedScene _foxScene = GD.Load<PackedScene>("res://scenes/fox.tscn");
	private PackedScene _jayhawkScene = GD.Load<PackedScene>("res://scenes/jayhawk.tscn");
	private PackedScene _jayhawkTwoScene = GD.Load<PackedScene>("res://scenes/jayhawktwo.tscn");
	private Path2D _path;
	private float _spawnTimer = 0f;
	private float _spawnInterval = 2f;

	private uint heartsLeft = 10;
	private uint selectedTroop;
	private void OnFoxReachedEnd()
	{
		heartsLeft -= 1;
		GD.Print("health remaining: ", heartsLeft);
		
		if (heartsLeft <= 0)
		{
			GD.Print("game over");
		}
	}
	public override void _Input(InputEvent @event)
{
	if (@event is InputEventMouseButton mouse && 
		mouse.ButtonIndex == MouseButton.Left && 
		mouse.Pressed)
	{
		PlaceTroop(GetGlobalMousePosition());
	}
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
	GD.Print("trying this at least");
	var jayhawk = _jayhawkScene.Instantiate<JayHawkBasic>();
	jayhawk.Position = position;
	AddChild(jayhawk);
	break;
case 2:
	var jayhawkTwo = _jayhawkTwoScene.Instantiate<JayHawkTwo>();
	jayhawkTwo.Position = position;
	AddChild(jayhawkTwo);
	break;
	}
	}

	public override void _Ready()
	{
			GetNode<Button>("CanvasLayer/Troop0").Pressed += () => selectedTroop = 1;
	GetNode<Button>("CanvasLayer/Troop1").Pressed += () => selectedTroop = 2;
	GetNode<Button>("CanvasLayer/Troop2").Pressed += () => selectedTroop = 3;
	GetNode<Button>("CanvasLayer/Troop3").Pressed += () => selectedTroop = 4;
	GetNode<Button>("CanvasLayer/Troop4").Pressed += () => selectedTroop = 5;//sorry for the number switch ballers
	

		_path = GetNode<Path2D>("GamePath");
		GD.Print("path found: ", _path.Name);
	}

	public override void _Process(double delta)
	{
		_spawnTimer -= (float)delta;
		if (_spawnTimer <= 0)
		{
			SpawnFox();
			_spawnTimer = _spawnInterval;
		}
	}

	private void SpawnFox()
	{
		var fox = _foxScene.Instantiate<Enemy>();
		fox.Scale = new Vector2(0.5f, 0.5f); //just a simple scale down
		fox.ReachedEnd += OnFoxReachedEnd; // subscribe to signal
		fox.Loop = false;
		_path.AddChild(fox);
		GD.Print("fox spawned");
	}
}
