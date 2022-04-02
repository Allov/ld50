using Godot;
using System;

public class Projectile : KinematicBody2D
{
    [Export] public float Damage = 10f;

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Export] public float MaxSpeed = 30f;
    [Export] public Curve Speed;
    private Vector2 Velocity;
    public float Acceleration;
    [Export] public float AccelerationFactor = 0.01f;
    private Particles2D Explosion;
    private AnimationPlayer AnimationPlayer;
    [Export] public int Cost = 10000;

    public event EventHandler OnExploded;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Explosion = GetNode<Particles2D>("Explosion");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _PhysicsProcess(float delta)
    {
        Acceleration = Mathf.Clamp(Acceleration + (AccelerationFactor + delta), 0f, 1f);

        Velocity = Vector2.Up * (MaxSpeed * Speed.Interpolate(Acceleration));
        Velocity = MoveAndSlide(Velocity);
    }

    public void _on_Area2D_body_entered(Node node)
    {
        AnimationPlayer.Play("Explode");
        Explosion.OneShot = true;
        Explosion.Emitting = true;
        Explosion.Visible = true;

        OnExploded?.Invoke(this, EventArgs.Empty);
    }

}
