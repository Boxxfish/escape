using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Handles overall selection for items.
/// </summary>
public class ItemSelector : Spatial
{
	public const float RAY_LEN = 100.0f;

	[Export]
	public NodePath camPath;
	[Export]
	public NodePath invPath;
	[Export]
	public NodePath examPath;

	private Camera cam;
	private Inventory inv;
	private ItemExaminer exam;

	private List<InteractiveItem> interItems = new List<InteractiveItem>();
	private InteractiveItem targetItem;
	private Vector2 mousePos;
	private bool mouseMoved;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		this.cam = this.GetNode<Camera>(this.camPath);
		this.inv = this.GetNode<Inventory>(this.invPath);
		this.exam = this.GetNode<ItemExaminer>(this.examPath);
	}

	// Registers an interactive item.
	public void RegisterItem(InteractiveItem item) {
		this.interItems.Add(item);
		item.Inventory = this.inv;
		item.Examiner = this.exam;
	}

	// Deregisters an interactive item.
	public void DeregisterItem(InteractiveItem item) {
		this.interItems.Remove(item);
		item.Inventory = null;
		item.Examiner = null;
	}

	// Called when input events happen.
	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseMotion eventMouseMotion) {
			// Save mouse position and set move flag.
			this.mousePos = eventMouseMotion.Position;
			this.mouseMoved = true;
		}
		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.ButtonIndex == (int)ButtonList.Left && eventMouseButton.Pressed) {
			// Select item if target is not null
			if (this.targetItem != null)
				this.targetItem._on_InteractiveItem_Selected();
		}
	}

	// Called when a physics step occurs.
	public override void _PhysicsProcess(float delta) {
		if (this.mouseMoved) {
			// Perform raycasting
			Vector3 from = this.cam.ProjectRayOrigin(this.mousePos);
			Vector3 to = from + this.cam.ProjectRayNormal(this.mousePos) * RAY_LEN;
			Godot.Collections.Dictionary rayDict = this.GetWorld().DirectSpaceState.IntersectRay(from, to);
			if (rayDict.Count > 0) {
				Godot.Object newTarget = (Godot.Object)rayDict["collider"];

				// If newTarget is not targetItem, call events on both items
				if (newTarget != this.targetItem) {
					if (newTarget is InteractiveItem && this.interItems.Contains((InteractiveItem)newTarget))
						((InteractiveItem)newTarget)._on_InteractiveItem_Enter();
					if (this.targetItem != null)
						this.targetItem._on_InteractiveItem_Leave();
				}

				// If newTarget is an interactive item, set it as the target
				if (newTarget is InteractiveItem && this.interItems.Contains((InteractiveItem)newTarget))
					this.targetItem = (InteractiveItem)newTarget;
				else
					this.targetItem = null;
			}
			else {
				// Call leave event on target item
				if (this.targetItem != null)
					this.targetItem._on_InteractiveItem_Leave();

				// Set the current target to null
				this.targetItem = null;
			}

			this.mouseMoved = false;
		}
	}
}
