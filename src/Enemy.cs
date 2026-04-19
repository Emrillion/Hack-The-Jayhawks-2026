using Godot;
using System;
using System.ComponentModel;

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
public partial class Enemy : PathFollow2D
{
    public enum EnemyType
    {
        Red, 
        Blue,  
        Green,
        Grey
    };
    public EnemyType Type { get; private set; }
    [Export] public float Speed = 100f;             // Units per second
    [Export] public int MaxHealth;            // Max health
    [Export] private int _currentHealth;

    // Emitted when an Area2D or PhysicsBody2D enters the child Area2D
    [Signal] public delegate void HitDetectedEventHandler(Node body);
    [Signal] public delegate void ReachedEndEventHandler();

    // Different textures
    [Export] public Texture2D Red_Fox { get; set; }
    [Export] public Texture2D Green_Fox { get; set; }
    [Export] public Texture2D Blue_Fox { get; set; }
    [Export] public Texture2D Grey_Fox { get; set; }
    private Sprite2D _sprite;

    private Area2D _area;
    public override void _Ready()
    {
        Type = MaxHealth switch
        {
            <= 5 => EnemyType.Red,
            <= 10 => EnemyType.Blue,
            <= 15 => EnemyType.Green,
            _ => EnemyType.Grey
        };

        _sprite = GetNode<Sprite2D>("Sprite2D");
        _currentHealth = MaxHealth;

        // Makes it so the object doesn't rotate
        Rotates = false;

        // Makes it so the object doesn't loop around the path.
        Loop = false;

        // Assigning textures

        Red_Fox = GD.Load<Texture2D>("res://assets/sprites/fox.png");
        Green_Fox = GD.Load<Texture2D>("res://assets/sprites/green_fox.png");
        Blue_Fox = GD.Load<Texture2D>("res://assets/sprites/blue_fox.png");
        Grey_Fox = GD.Load<Texture2D>("res://assets/sprites/grey_fox.png");

        UpdateSprite();

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

    // Runs for the event of the game starting
    public override void _Process(double delta)
    {
        float direction = 1f;
        Progress += Speed * direction * (float)delta;

        if (ProgressRatio >= 1.0f)
        {
            OnReachedEnd();
        }
    }

    // End state
    private void OnReachedEnd()
    {
        GD.Print("Enemy reached the end!");
        // Add code to make the player take damage
        EmitSignal(SignalName.ReachedEnd);
        // Kind of deletes the object
        QueueFree();
    }

    // Sprite Updating

    private void UpdateSprite()
    {
        if (_sprite == null) return;

        _sprite.Texture = _currentHealth switch
        {
            >= 20 => Grey_Fox,
            > 10 => Green_Fox,
            > 5  => Blue_Fox,
            _    => Red_Fox
        };

    }

    // --- Collision handlers ---

    private void OnBodyEntered(Node body)
    {
        GD.Print($"PathFollower hit: {body.Name}");
        EmitSignal(SignalName.HitDetected, body);
    }

    private void OnAreaEntered(Area2D area)
    {
        GD.Print($"PathFollower overlapped area: {area.Name}");
        EmitSignal(SignalName.HitDetected, area);

        // TO DO:
        //      TakeDamage(Feather's DamageValue)
    }

    // Called when taking damage
    private void TakeDamage(int DamageValue)
    {
        _currentHealth -= DamageValue;
        UpdateSprite();
    }
}