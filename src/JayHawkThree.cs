using Godot;
using System;
using System.ComponentModel;

public partial class JayHawkThree: AnimatableBody2D
{
	private float fireDelay = 0f;
	private float fireRate = 3;
	private PackedScene _featherScene;
	private Node2D _currentTarget;
	public int upgradeLevel;
	public override void _Ready()
	{
		_featherScene = GD.Load<PackedScene>("res://scenes/feather.tscn");
		if (_featherScene == null) { GD.Print("PATH IS NULL ERROR"); }

		var area = GetNode<Area2D>("AreaObj");
		area.AreaEntered += OnAreaEntered;
		area.AreaExited += OnAreaExited;
		GD.Print("signal hooked");
	}
public void Upgrade()
{
	upgradeLevel++;
	switch (upgradeLevel)
	{
		case 1:
			fireRate = 4;
			GD.Print("upgraded fire rate");
			break;
		case 2:
			fireRate = 3;
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
		GD.Print("area entered: " + area.Name);
		if (area.Name == "Fox")
			_currentTarget = area.GetParent<Node2D>();
	}

	private void OnAreaExited(Area2D area)
	{
		if (area.Name == "Fox")
			_currentTarget = null;
	}

	public override void _Process(double delta)
	{
		fireDelay -= (float)delta;

		if (_currentTarget != null && IsInstanceValid(_currentTarget) && fireDelay <= 0)
		{
			fireDelay = fireRate;
			var feather = _featherScene.Instantiate<Feather>();
			feather.damage = 7;
			feather.Init(_currentTarget);
			GetParent().AddChild(feather);
			feather.GlobalPosition = GlobalPosition;
			GD.Print("fired feather");
		}
	}
}
