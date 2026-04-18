using Godot;
using System;
using System.Collections;
using System.ComponentModel;

public partial class Feather : AnimatableBody2D
{
    private Node2D _target;
    private float _speed = 100f;

    public override void _Ready()
    {
        GD.Print("feather ready");
        var area = GetNode<Area2D>("FeatherArea2D");
        area.AreaEntered += OnAreaEntered;
        area.BodyExited += OnBodyExited;
    }

    public void Init(Node2D target)
    {
        _target = target;
    }

public override void _PhysicsProcess(double delta)
{
    if (_target == null || !IsInstanceValid(_target))
    {
        QueueFree();
        return;
    }
    Vector2 direction = (_target.GlobalPosition - GlobalPosition).Normalized();
    MoveAndCollide(direction * _speed * (float)delta);
    Rotation = direction.Angle() + Mathf.Pi / 2;
}

    private void OnAreaEntered(Area2D area)
    {
    GD.Print("area parent name: " + area.GetParent().Name);
    if (area.GetParent().Name == "Fox")
    {
        area.GetParent().QueueFree();
        QueueFree();
    }
    }

    private void OnBodyExited(Node2D body)
    {
        ;
    }
}