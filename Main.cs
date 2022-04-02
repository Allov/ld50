using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Main : Node2D
{
    private const float InGameDaySeconds = (600f / 10f / 60f);
    private const float MissileTimeAddValue = 1f;
    private const int ReportFrequencyTime = 10;
    private TileMap TileMap;

    public Timer TimeUntilDoomTimer { get; private set; }
    [Export] public PackedScene TurretScene;

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Export] public float TimeUntilDoom = 600f; // in realtime minutes
    private Label TimeUntilDoomLabel;
    private List<int> Columns;
    private Label MoneyValueLabel;
    private Button TurretButton;
    private Button FireButton;
    private Label DaysSavedValueLabel;
    public float DifferentialFromLastSampleTimeUntilDoom;

    private float StartTimeUntilDoom;
    private float DaysSaved;
    private List<Turret> Turrets = new List<Turret>();
    [Export] public int ColumnCount = 10;
    [Export] public int GridSpace = 16;
    [Export] public long Money = 100000000;
    private int ElapsedRealTime;
    private int MissileExplodedCount;
    private int MissileExplodedCountTotal;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        TileMap = GetNode<TileMap>("Universe/TileMap");
        TimeUntilDoomTimer = new Timer();
        AddChild(TimeUntilDoomTimer);

        TimeUntilDoomTimer.WaitTime = 1f;
        TimeUntilDoomTimer.Start();
        TimeUntilDoomTimer.Connect("timeout", this, nameof(_on_TimeUntilDoomTicked));
        TimeUntilDoomLabel = GetNode<Label>("UI/ImpendingDoomLabel");

        Columns = GD.Range(ColumnCount).ToList();

        MoneyValueLabel = GetNode<Label>("UI/VBoxContainer/Info/VBoxContainer/Money/MoneyValueLabel");
        TurretButton = GetNode<Button>("UI/VBoxContainer/Military/VBoxContainer/GridContainer/TurretButton");
        FireButton = GetNode<Button>("UI/FireButton");

        DaysSavedValueLabel = GetNode<Label>("UI/VBoxContainer/Info/VBoxContainer/DaysSaved/DaysSavedValueLabel");

        StartTimeUntilDoom = TimeUntilDoom;
    }

    public override void _Process(float delta)
    {
        TimeUntilDoomLabel.Text = $"{TimeUntilDoom.Map(0f, 600f, 0, 10f).ToString("n1")} until Impending Doom!";
        MoneyValueLabel.Text = $"{String.Format("{0:n0}", Money)}$";
        DaysSavedValueLabel.Text = $"{DaysSaved}";



        FireButton.Disabled = Money < Turrets.Count * 10000;
        TurretButton.Disabled = Money < 50000;
    }

    private void _on_TimeUntilDoomTicked()
    {
        ElapsedRealTime += 1;
        TimeUntilDoom = TimeUntilDoom - MissileTimeAddValue;

        if (ElapsedRealTime % ReportFrequencyTime == 0)
        {
            DifferentialFromLastSampleTimeUntilDoom = StartTimeUntilDoom - TimeUntilDoom;
            StartTimeUntilDoom = TimeUntilDoom;

            DaysSaved = InGameDaySeconds * (InGameDaySeconds * (MissileTimeAddValue / (float)ReportFrequencyTime));

            MissileExplodedCount = 0;

        }
    }

    public void _on_TurretButton_pressed()
    {
        if (Columns.Count <= 0) return;

        var turret = TurretScene.Instance() as Turret;
        turret.PlayerControlled = true;
        var colIndex = (int)GD.RandRange(0f, (float)Columns.Count);
        var column = Columns[colIndex];

        turret.GlobalPosition = new Vector2(100f + column * GridSpace, 216f / 2f);
        GetNode<Node2D>("Universe").AddChild(turret);
        Turrets.Add(turret);

        turret.OnMissileExploded += OnMissileExploded;


        Columns.RemoveAt(colIndex);

        Money -= turret.Cost;
    }

    private void OnMissileExploded(object sender, EventArgs e)
    {
        TimeUntilDoom += 1f;
        MissileExplodedCount++;
        MissileExplodedCountTotal++;
    }

    public void _on_FireButton_pressed()
    {
        foreach (var turret in Turrets)
        {
            Money -= turret.Fire();
        }
    }

}
