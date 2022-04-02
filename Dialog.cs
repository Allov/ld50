using Godot;
using System;

public class Dialog : Control
{
    private AudioStreamPlayer BibSound;
    private RichTextLabel RichTextLabel;
    private string Text;
    private int CurrentCharacterIndex;
    private float TypeTimer;
    private OpenSimplexNoise Noise;
    [Export] public float TypeTimeDelay = .05f;

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BibSound = GetNode<AudioStreamPlayer>("BipSound");
        RichTextLabel = GetNode<RichTextLabel>("PanelContainer/HBoxContainer/RichTextLabel");
        
        Text = RichTextLabel.BbcodeText;
        RichTextLabel.BbcodeText = "";
        CurrentCharacterIndex = 0;
        TypeTimer = 0f;

        Noise = new OpenSimplexNoise();
        Noise.Period = 512;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (!Visible) return;

        if (CurrentCharacterIndex >= Text.Length) return;

        TypeTimer += delta;

        if (TypeTimer >= TypeTimeDelay)
        {
            var t = Text.Substring(CurrentCharacterIndex, 1);
            TypeTimer = 0f;
            RichTextLabel.AppendBbcode(t);
            CurrentCharacterIndex++;
            
            var noise = Noise.GetNoise1d(CurrentCharacterIndex);
            BibSound.PitchScale = noise.Map(-1f, 1f, .7f, 1f);
            BibSound.Play();
        }
    }
}

