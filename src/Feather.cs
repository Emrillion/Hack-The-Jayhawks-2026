using Godot;
using System;
using System.Collections;
using System.ComponentModel;

public partial class Feather: AnimatableBody2D 
{
	// Called when the node enters the scene tree for the first time.
	//let's init ours
    [Export] public float directionX;
    [Export] public float directionY;
	public override void _Ready()
	{
		GD.Print("hello");
		var area = GetNode<Area2D>("FeatherArea2D");
        area.BodyEntered += OnBodyEntered;//awful c# syntax to
        area.BodyExited += OnBodyExited;//subscribe to events

	}
    private void OnBodyEntered(Node2D body)
	{
		GD.Print(body.Name);
		if (body.Name == "Fox")
		{
            QueueFree();//queuefrees itself
		}
	}

    private void OnBodyExited(Node2D body)
	{
		;//implement later
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        if (directionX > Position.X)
        {
            Position = new Godot.Vector2(Position.X + (float) delta, Position.Y);
        }
        if (directionX < Position.X) {
            Position = new Godot.Vector2(Position.X - (float) delta, Position.Y);
        }

        if (directionY > Position.X)
        {
            Position = new Godot.Vector2(Position.X , Position.Y + (float) delta);
        }
        if (directionX < Position.X) {
            Position = new Godot.Vector2(Position.X, Position.Y - (float) delta);
        }
        
        
    }
}
