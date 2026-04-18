using Godot;
using System;
using System.ComponentModel;

public partial class JayHawkBasic : AnimatableBody2D 
{
	// Called when the node enters the scene tree for the first time.
	private float fireDelay =- 1;
	private PackedScene _featherScene;
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
private void OnAreaEntered(Area2D area)
{
    GD.Print(area.Name);
    if (area.Name == "Fox")
    {
        GD.Print("made feather");
        var feather = _featherScene.Instantiate<Feather>();
        feather.Init(area.GetParent<Node2D>()); // pass the fox node
        GetParent().AddChild(feather);
        feather.GlobalPosition = GlobalPosition; // spawn at jayhawk
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
