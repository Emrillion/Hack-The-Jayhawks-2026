using Godot;
using System;
using System.ComponentModel;

public partial class JayHawkTwo : AnimatableBody2D
{
	private float fireDelay = 0f;
	private float fireRate = 0.2f;
	private PackedScene _featherScene;
	private Node2D _currentTarget;

	public override void _Ready()
	{
		_featherScene = GD.Load<PackedScene>("res://scenes/feather.tscn");
		if (_featherScene == null) { GD.Print("PATH IS NULL ERROR"); }
		GD.Print("JayHawkTwo ready");

		var area = GetNode<Area2D>("AreaObj");
		area.AreaEntered += OnAreaEntered;
		area.AreaExited += OnAreaExited;
		GD.Print("signal hooked");
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
			feather.damage = 1;
			feather.Init(_currentTarget);
			GetParent().AddChild(feather);
			feather.GlobalPosition = GlobalPosition;
			GD.Print("fired feather");
		}
	}
}
