using Godot;
using System.Collections.Generic;

public partial class WaveSpawner : Node2D
{
    [Export] public PackedScene EnemyScene { get; set; }
    [Export] public NodePath Path2DPath { get; set; }
    [Export] public float TimeBetweenWaves { get; set; } = 20.0f;

    [Signal] public delegate void CountdownUpdatedEventHandler(float timeRemaining);
    [Signal] public delegate void WaveStartedEventHandler(int waveNumber);
    [Signal] public delegate void AllWavesCompleteEventHandler();

    private Path2D _path2D;

    private struct WaveData
    {
        public int Count;
        public float Interval;
    }

    private List<WaveData> _waves = new()
    {
        // WAVES
        new WaveData { Count = 5,  Interval = 2f }, // 1
        new WaveData { Count = 10, Interval = 2f }, // 2
        new WaveData { Count = 10, Interval = 1.5f }, // 3
        new WaveData { Count = 15, Interval = 1.5f }, // 4
        new WaveData { Count = 10, Interval = 0.8f }, // 5
        new WaveData { Count = 20, Interval = 1.5f }, // 6 
        new WaveData { Count = 15, Interval = 1.0f }, // 7
        new WaveData { Count = 20, Interval = 1.0f }, // 8
        new WaveData { Count = 15, Interval = 1.0f }, // 9
        new WaveData { Count = 40, Interval = 1.0f }, // 10
    };

    private int _currentWave = 0;
    private int _enemiesRemaining = 0;
    private bool _isSpawning = false;

    // Countdown state
    private bool _isCountingDown = false;
    private bool _countdownPaused = false;
    private float _countdownTimeRemaining = 0f;
    private int _nextWaveIndex = 0;

    public override void _Ready()
    {
        _path2D = GetNode<Path2D>(Path2DPath);
        StartWave(0);
    }

    public override void _Process(double delta)
    {
        // Tick the countdown down manually so we can pause it
        if (_isCountingDown && !_countdownPaused)
        {
            _countdownTimeRemaining -= (float)delta;
            EmitSignal(SignalName.CountdownUpdated, _countdownTimeRemaining);

            if (_countdownTimeRemaining <= 0f)
            {
                _isCountingDown = false;
                StartWave(_nextWaveIndex);
            }
        }
    }

    // Call this from a UI button to pause the countdown
    public void PauseCountdown()
    {
        if (!_isCountingDown)
        {
            GD.Print("No countdown is active.");
            return;
        }
        _countdownPaused = true;
        GD.Print("Countdown paused.");
    }

    // Call this from a UI button to resume the countdown
    public void ResumeCountdown()
    {
        if (!_isCountingDown)
        {
            GD.Print("No countdown is active.");
            return;
        }

        _countdownPaused = false;
        GD.Print("Countdown resumed.");
    }

    // Call this to skip the countdown and start the next wave immediately
    public void SkipCountdown()
    {
        if (!_isCountingDown)
        {
            GD.Print("No countdown is active.");
            return;
        }

        _isCountingDown = false;
        _countdownPaused = false;
        GD.Print("Countdown skipped!");
        StartWave(_nextWaveIndex);
    }

    public void StartWave(int waveIndex)
    {
        if (waveIndex >= _waves.Count)
        {
            GD.Print("All waves complete!");
            EmitSignal(SignalName.AllWavesComplete);
            return;
        }

        if (_isSpawning)
        {
            GD.PrintErr("A wave is already in progress!");
            return;
        }

        _currentWave = waveIndex;
        _enemiesRemaining = _waves[waveIndex].Count;
        _isSpawning = true;

        GD.Print($"Starting wave {_currentWave + 1} — {_enemiesRemaining} enemies incoming!");
        EmitSignal(SignalName.WaveStarted, _currentWave + 1);
        ScheduleNextSpawn();
    }

    private void ScheduleNextSpawn()
    {
        var timer = GetTree().CreateTimer(_waves[_currentWave].Interval);
        timer.Timeout += OnSpawnTimer;
    }

    private void OnSpawnTimer()
    {
        if (_enemiesRemaining > 0)
        {
            SpawnEnemy();
            _enemiesRemaining--;
            ScheduleNextSpawn();
        }
        else
        {
            OnWaveComplete();
        }
    }

    private void SpawnEnemy()
    {
        if (EnemyScene == null)
        {
            GD.PrintErr("EnemyScene is not assigned!");
            return;
        }

        if (_path2D == null)
        {
            GD.PrintErr("Path2D is not assigned!");
            return;
        }

        var enemy = EnemyScene.Instantiate<Enemy>();
        enemy.Scale = new Vector2(0.5f, 0.5f);
        enemy.Progress = 0f;
        _path2D.AddChild(enemy);
    }

    private void OnWaveComplete()
    {
        _isSpawning = false;
        GD.Print($"Wave {_currentWave + 1} complete!");

        int nextWave = _currentWave + 1;
        if (nextWave < _waves.Count)
        {
            ScheduleNextWave(nextWave);
        }
        else
        {
            GD.Print("All waves complete!");
            EmitSignal(SignalName.AllWavesComplete);
        }
    }

    private void ScheduleNextWave(int nextWaveIndex)
    {
        GD.Print($"Next wave in {TimeBetweenWaves} seconds...");
        _nextWaveIndex = nextWaveIndex;
        _countdownTimeRemaining = TimeBetweenWaves;
        _isCountingDown = true;
        _countdownPaused = false;
    }
}