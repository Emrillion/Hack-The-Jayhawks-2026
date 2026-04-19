using System.Security.Cryptography.X509Certificates;
using Godot;

public partial class Main_Test : Node2D
{
    private PackedScene _foxScene = GD.Load<PackedScene>("res://scenes/Enemy.tscn");
    private Path2D _path;
    private float _spawnInterval = 2f;

    private Label countdownLabel;

    private uint heartsLeft = 10;
    private uint selectedTroop;
    public override void _Ready()
    {
        var _spawner = GetNode<WaveSpawner>("WaveSpawner");
        countdownLabel = GetNode<Label>("CanvasLayer/CountdownLabel");
        countdownLabel.Text = "Wave 1 started!";

        if (_spawner == null)
        {
            GD.PrintErr("WaveSpawner not found!");
            return;
        }

        if (countdownLabel == null)
        {
            GD.PrintErr("CountdownLabel not found!");
            return;
        }

        // Wire up buttons
        GetNode<Button>("CanvasLayer/Pause").Pressed   += _spawner.PauseCountdown;
        GetNode<Button>("CanvasLayer/Resume").Pressed  += _spawner.ResumeCountdown;
        GetNode<Button>("CanvasLayer/SkipButton").Pressed += _spawner.SkipCountdown;

        // Wire up spawner signals
        _spawner.CountdownUpdated += OnCountdownUpdated;
        _spawner.WaveStarted      += OnWaveStarted;
        _spawner.AllWavesComplete += OnAllWavesComplete;

        _path = GetNode<Path2D>("GamePath");
        GD.Print("path found: ", _path.Name);
    }

    private void OnCountdownUpdated(float timeRemaining)
    {
        countdownLabel.Text = $"Next wave in: {timeRemaining:F1}s";
    }

    private void OnWaveStarted(int waveNumber)
    {
        countdownLabel.Text = $"Wave {waveNumber} started!";
    }

    private void OnAllWavesComplete()
    {
        countdownLabel.Text = "All waves complete!";
    }

    private void OnFoxReachedEnd()
    {
        heartsLeft -= 1;
        GD.Print("health remaining: ", heartsLeft);
        
        if (heartsLeft <= 0)
        {
            GD.Print("game over");
        }
    }
}