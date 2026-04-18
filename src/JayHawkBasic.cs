using Godot;
using System;
using System.ComponentModel;

public partial class JayHawkBasic : AnimatableBody2D 
{
	// Called when the node enters the scene tree for the first time.
	private float fireDelay =- 1;
	private PackedScene _foxScene;
	//let's init ours
	public override void _Ready()
	{
		_foxScene = GD.Load<PackedScene>("res://src/JayHawkBasic.tscn");
		GD.Print("hello");
		var area = GetNode<Area2D>("AreaObj");
        area.BodyEntered += OnBodyEntered;//awful c# syntax to
        area.BodyExited += OnBodyExited;//subscribe to events

	}
    private void OnBodyEntered(Node2D body)
	{
		GD.Print(body.Name);
		if (body.Name == "Fox")
		{
			body.QueueFree();//delete that fox next frame
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
