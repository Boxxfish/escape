using Godot;
using System;

public class ItemExaminer : Position3D
{
	public const string ITEMS_PATH = "res://ItemSystem/Items/";
	public const string TSCN_SUFFIX = ".tscn";
	public const float ROT_AMOUNT = 0.005f;
	public const float ROT_INTER = 0.4f;

	[Export]
	public NodePath closeButtonPath;
	[Export]
	public NodePath examScreenPath;
	[Export]
	public NodePath camPath;

	private bool canRotate;
	private Camera cam;
	private Quat targetQuat;
	private Button closeButton;
	private ColorRect examScreen;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		this.targetQuat = this.Transform.basis.Quat();
		this.closeButton = this.GetNode<Button>(this.closeButtonPath);
		this.examScreen = this.GetNode<ColorRect>(this.examScreenPath);
		this.cam = this.GetNode<Camera>(this.camPath);

		// Connect signals
		this.closeButton.Connect(Signals.PRESSED, this, nameof(_on_CloseButton_Pressed));
	}

	// Called on every time step.
	public override void _Process(float delta) {
		Quat interQuat = this.Transform.basis.Quat().Slerp(this.targetQuat, ROT_INTER);
		this.Transform = new Transform(new Basis(interQuat), this.Transform.origin);
	}

	// Called when input events occur.
	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseMotion eventMouseMotion && Input.IsMouseButtonPressed((int)ButtonList.Left)) {
			if (this.canRotate) {
				float xAmount = eventMouseMotion.Relative.x * ROT_AMOUNT;
				float yAmount = eventMouseMotion.Relative.y * ROT_AMOUNT;
				Quat xQuat = new Quat(Vector3.Up, xAmount);
				Quat yQuat = new Quat(Vector3.Right, yAmount);
				this.targetQuat = (yQuat * xQuat * this.Transform.basis.Quat()).Normalized();
			}
		}
	}

	// Sets the examiner's item.
	public void SetItem(ItemInfo itemInfo) {
		// Reset rotation
		this.targetQuat = Quat.Identity;
		this.Transform = new Transform(Basis.Identity, this.Transform.origin);

		// Remove the previous item, if applicable
		if (this.GetChildCount() > 0) {
			Node prevChild = this.GetChild(0);
			this.RemoveChild(prevChild);
			prevChild.Free();
		}

		// Instantiate the item and add it as a child
		PackedScene scn = GD.Load<PackedScene>(ITEMS_PATH + itemInfo.ItemID + TSCN_SUFFIX); ;
		InteractiveItem item = (InteractiveItem)scn.Instance();
		this.AddChild(item);
		item.SetCam(this.cam);

		// Resize and center the item using the collider as bounds
		CollisionShape collider = item.GetNode<CollisionShape>("Collider");
		BoxShape colliderShape = (BoxShape)collider.Shape;
		float largestDim = Mathf.Max(Mathf.Max(colliderShape.Extents.x, colliderShape.Extents.y), colliderShape.Extents.z);
		item.Scale = Vector3.One * (1.0f / largestDim);
		item.Translate(-collider.Transform.origin);

		// Set examiner flags
		this.canRotate = item.canPickUp;

		// Add UI elements
		this.examScreen.Visible = true;
		this.closeButton.Visible = true;
	}

	// Called when close button is pressed.
	public void _on_CloseButton_Pressed() {
		// Remove current item
		this.RemoveChild(this.GetChild(0));

		// Hide UI elements
		this.examScreen.Visible = false;
		this.closeButton.Visible = false;
	}
}
