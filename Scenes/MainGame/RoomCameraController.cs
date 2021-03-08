using Godot;
using System;

public enum Direction {
	UP,
	DOWN,
	LEFT,
	RIGHT
}

public class RoomCameraController : Camera
{
	public const float ROT_SPD = 20.0f;
	
	private bool moving;
	private Direction lookingAt;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)  {
		if (this.moving) {
			// Calculate where we should be looking
			float targetRot = 0.0f;
			if (this.lookingAt == Direction.UP)
				targetRot = 0.0f;
			else if (this.lookingAt == Direction.DOWN)
				targetRot = 180.0f;
			else if (this.lookingAt == Direction.LEFT)
				targetRot = 90.0f;
			else
				targetRot = 270.0f;

			// Check if we are at the target
			Quat oldQuat = new Quat(this.GlobalTransform.basis);
			Quat newQuat = new Quat(Vector3.Up, Mathf.Deg2Rad(targetRot));
			if (oldQuat.IsEqualApprox(newQuat))
				this.moving = false;
			else
				// Otherwise, rotate camera towards target
				this.GlobalTransform = new Transform(oldQuat.Slerp(newQuat, ROT_SPD * delta), this.GlobalTransform.origin);

		}
	}
	
	// Rotates the camera left or right one turn.
	public void RotCam(Direction dir) {
		this.moving = true;
		if (dir == Direction.LEFT) {
			if (this.lookingAt == Direction.UP)
				this.lookingAt = Direction.LEFT;
			else if (this.lookingAt == Direction.DOWN)
				this.lookingAt = Direction.RIGHT;
			else if (this.lookingAt == Direction.LEFT)
				this.lookingAt = Direction.DOWN;
			else
				this.lookingAt = Direction.UP;
		}
		else if (dir == Direction.RIGHT) {
			if (this.lookingAt == Direction.UP)
				this.lookingAt = Direction.RIGHT;
			else if (this.lookingAt == Direction.DOWN)
				this.lookingAt = Direction.LEFT;
			else if (this.lookingAt == Direction.RIGHT)
				this.lookingAt = Direction.DOWN;
			else
				this.lookingAt = Direction.UP;
		}
	}
}
