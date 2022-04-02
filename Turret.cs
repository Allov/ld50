using Godot;
using System;

public class Turret : KinematicBody2D
{
    [Export] public PackedScene ProjectileScene;
    [Export] public float FireRate = 1f;

    private Cooldown FireCooldown;


    [Export] public bool PlayerControlled = false;

    public int Cost = 50000;

    public event EventHandler OnMissileExploded;

    public override void _Ready()
    {
        FireCooldown = new Cooldown(FireRate, this);
    }

    public override void _Process(float delta)
    {
        if ((!PlayerControlled && FireCooldown.Use()))
        {
            CreateProjectile();
        }
    }

    private int CreateProjectile()
    {
        var p = ProjectileScene.Instance() as Projectile;
        p.GlobalPosition = GlobalPosition + Vector2.Left.Rotated(Mathf.Pi * 2f / (float)GD.RandRange(1f, 4f)) * 5f;
        GetTree().Root.AddChild(p);

        p.OnExploded += OnExploded;

        return p.Cost;
    }

    private void OnExploded(object sender, EventArgs e)
    {
        OnMissileExploded?.Invoke(this, EventArgs.Empty);
    }

    public int Fire()
    {
        if (FireCooldown.Use())
        {
            return CreateProjectile();
        }

        return 0;
    }
}
