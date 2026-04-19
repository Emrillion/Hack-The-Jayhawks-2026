using Godot;
using System;
using System.ComponentModel;

public partial class JayHawkBasic : AnimatableBody2D 
{
	// Called when the node enters the scene tree for the first time.
	private float fireRate = 1;
	private float fireDelay =- 1;
	private PackedScene _featherScene;
	private uint upgradeLevel;
	//let's init ours
	public override void _Ready()
	{
		_featherScene = GD.Load<PackedScene>("res://scenes/feather.tscn");
		if (_featherScene == null){GD.Print("PATH IS NULLL ERROR");}
		GD.Print("hello");
		var area = GetNode<Area2D>("AreaObj");
		area.AreaEntered += OnAreaEntered;//awful c# syntax to
			GD.Print("signal hooked");
	}
public void Upgrade()
{
	upgradeLevel++;
	switch (upgradeLevel)
	{
		case 1:
			fireRate = 2;
			GD.Print("upgraded fire rate");
			break;
		case 2:
			fireRate = 1;
			GD.Print("upgraded fire rate again");
			break;
		case 3:
			// pass damage to feather
			GD.Print("max upgrade");
			break;
	}
}
private void OnAreaEntered(Area2D area)
{
	GD.Print(area.Name);
	if (area.Name == "Fox") // only fire for fox areas
	{
		if (fireDelay > 0) return; // don't fire if on cooldown
		
		GD.Print("made feather");
		fireDelay = fireRate; // 1 second cooldown
		
		var feather = _featherScene.Instantiate<Feather>();
		feather.Init(area.GetParent<Node2D>());
		GetParent().AddChild(feather);
		feather.GlobalPosition = GlobalPosition;
	}
}

	private void OnBodyExited(Node2D body)
	{
		;//implement later
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		fireDelay -= (float) delta;
	}
}
