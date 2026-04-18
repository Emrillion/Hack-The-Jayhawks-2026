using Godot;
using System;

/// <summary>
/// Attach this script to a PathFollow2D node that is a child of a Path2D node.
/// The node will automatically move along the path at the set speed.
///
/// COLLISION SETUP:
/// Add an Area2D node as a child of this PathFollow2D, then add a CollisionShape2D
/// under the Area2D. The Area2D's body_entered / area_entered signals will fire
/// when something overlaps the follower. You can also set StopOnCollision = true
/// to pause movement automatically on contact.
/// </summary>
public partial class Pathfinding : PathFollow2D
{
    [Export] public float Speed = 100f;             // Units per second
    [Export] public bool Loop = false;              // Loop when reaching the end
    [Export] public bool Rotate = true;             // Rotate to face path direction
    [Export] public bool Reverse = false;           // Travel in reverse direction
    [Export] public bool StopOnCollision = false;   // Pause movement on collision

    // Emitted when an Area2D or PhysicsBody2D enters the child Area2D
    [Signal] public delegate void HitDetectedEventHandler(Node body);

    private Area2D _area;
    private bool _stopped = false;

    public override void _Ready()
    {
        this.Rotates = Rotate;
        this.Loop = Loop;

        // Look for a child Area2D to wire up collision detection
        _area = GetNodeOrNull<Area2D>("Area2D");

        if (_area != null)
        {
            // Fires when a PhysicsBody2D (e.g. CharacterBody2D) overlaps
            _area.BodyEntered += OnBodyEntered;
            // Fires when another Area2D overlaps
            _area.AreaEntered += OnAreaEntered;
        }
        else
        {
            GD.PushWarning("SimplePathFollower: No child Area2D found. " +
                           "Add an Area2D + CollisionShape2D child for collision support.");
        }
    }

    public override void _Process(double delta)
    {
        if (_stopped) return;

        float direction = Reverse ? -1f : 1f;
        Progress += Speed * direction * (float)delta;
    }

    // --- Collision handlers ---

    private void OnBodyEntered(Node body)
    {
        GD.Print($"PathFollower hit: {body.Name}");
        EmitSignal(SignalName.HitDetected, body);

        if (StopOnCollision)
            _stopped = true;
    }

    private void OnAreaEntered(Area2D area)
    {
        GD.Print($"PathFollower overlapped area: {area.Name}");
        EmitSignal(SignalName.HitDetected, area);

        if (StopOnCollision)
            _stopped = true;
    }

    // --- Public helpers ---

    /// <summary>Resume movement after a collision stop.</summary>
    public void Resume() => _stopped = false;

    /// <summary>Manually stop the follower.</summary>
    public void Stop() => _stopped = true;
}
/*
public partial class Pathfinding : PathFollow2D
{
    [Export] public float Speed = 100f;       // Units per second
    [Export] public bool Loop = false;        // Looping the path
    [Export] public bool Rotate = true;       // Rotate to face path direction
    [Export] public bool Reverse = false;     // Travel in reverse direction
    private int[] cur_pos;
    private int[] travel_points; 

    public override void _Ready()
    {
        this.Rotates = Rotate;
        this.Loop = Loop;
        cur_pos = [0,0]; // Change this to the starting position of the map
        travel_points = []; // Add points here that can be referenced later
        base._Ready();
    }

    public void travel(int travel_x, int travel_y)
    {
        if (cur_pos[0] < travel_x)
        {
            cur_pos = [cur_pos[0] + 1, cur_pos[1]];
        }
        else if (cur_pos[0] > travel_x)
        {
            cur_pos = [cur_pos[0] - 1, cur_pos[1]];
        }
        else if (cur_pos[1] < travel_y)
        {
            cur_pos = [cur_pos[0],cur_pos[1] + 1];
        }
        else if (cur_pos[1] > travel_y)
        {
            cur_pos = [cur_pos[0],cur_pos[1] - 1];
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }


}
*/