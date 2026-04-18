using Godot;
using System;

public partial class JayHawkBasic : CharacterBody2D 
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("hello");
		var area = GetNode<Area2D>("Area2D");
        area.BodyEntered += OnBodyEntered;//awful c# syntax to
        area.BodyExited += OnBodyExited;//subscribe to events

	}
    private void OnBodyEntered(Node2D body)
    {
        GD.Print(body.Name + " is in range");
    }

    private void OnBodyExited(Node2D body)
    {
        GD.Print(body.Name + " left range");
    }
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//we have to check the range 
		//let's check our body collision
	}
}
