using Godot;
using System;

/// <summary>
/// Performs an animation when clicked.
/// </summary>
public class AnimInteraction : ItemInteraction
{
	[Export]
	public string animName;
	[Export]
	public NodePath animPlayerPath;

	private AnimationPlayer animPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		this.animPlayer = this.GetNode<AnimationPlayer>(this.animPlayerPath);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		
	}

	// Called when icon is clicked.
	public override void OnClick() {
		this.animPlayer.Play(this.animName);
	}
}
