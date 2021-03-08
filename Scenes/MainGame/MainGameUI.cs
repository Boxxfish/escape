using Godot;
using System;

public class MainGameUI : Control
{
	[Export]
	public NodePath leftNavButtonPath;
	[Export]
	public NodePath rightNavButtonPath;
	[Export]
	public NodePath roomCamPath;

	private Button leftNavButton;
	private Button rightNavButton;
	private RoomCameraController roomCam;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Cache components
		this.leftNavButton = this.GetNode<Button>(this.leftNavButtonPath);
		this.rightNavButton = this.GetNode<Button>(this.rightNavButtonPath);
		this.roomCam = this.GetNode<RoomCameraController>(this.roomCamPath);

		// Wire signals
		this.leftNavButton.Connect(Signals.PRESSED, this, nameof(this._on_NavButton_Pressed), new Godot.Collections.Array { this.leftNavButton });
		this.rightNavButton.Connect(Signals.PRESSED, this, nameof(this._on_NavButton_Pressed), new Godot.Collections.Array { this.rightNavButton });
	}

	// Called when a navigation button is pressed.
	public void _on_NavButton_Pressed(Button button) {
		if (button == this.leftNavButton)
			this.roomCam.RotCam(Direction.LEFT);
		else if (button == this.rightNavButton)
			this.roomCam.RotCam(Direction.RIGHT);
	}
}
