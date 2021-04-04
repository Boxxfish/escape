using Godot;
using System;

public class ItemInteraction : StaticBody
{
	public const float RAY_LEN = 100.0f;

	private Vector2 mousePos;
	private bool mousePressed;
	private Camera cam;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		
	}

	// Sets the camera.
	public void SetCam(Camera cam) {
		this.cam = cam;
	}

	// Called when input happens.
	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.ButtonIndex == (int)ButtonList.Left && eventMouseButton.Pressed) {
			this.mousePressed = true;
			this.mousePos = eventMouseButton.Position;
		}
	}

	// Called during physics step.
	public override void _PhysicsProcess(float delta) {
		if (this.mousePressed) {
			Vector3 from = this.cam.ProjectRayOrigin(this.mousePos);
			Vector3 to = from + this.cam.ProjectRayNormal(this.mousePos) * RAY_LEN;
			Godot.Collections.Dictionary rayDict = this.GetWorld().DirectSpaceState.IntersectRay(from, to);
			if (rayDict.Count > 0) {
				Godot.Object newTarget = (Godot.Object)rayDict["collider"];
				// If clicked, calls click handler.
				if (this == newTarget)
					this.OnClick();
			}
			this.mousePressed = false;
		}
	}

	// Called when icon is clicked.
	public void OnClick() {

	}
}
