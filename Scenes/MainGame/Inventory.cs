using Godot;
using System;
using System.Collections.Generic;

public class Inventory : Node
{
	public const string ITEMS_PATH = "res://ItemSystem/Items/";
	public const string TSCN_SUFFIX = ".tscn";

	[Export]
	public NodePath itemObjsPath;
	[Export]
	public NodePath invButtonsPath;
	[Export]
	public NodePath examPath;
	[Export]
	public float itemSeparation;
	[Export]
	public float itemScale;

	private Spatial itemObjs;
	private Control invButtons;
	private ItemExaminer exam;
	private List<ItemInfo> items = new List<ItemInfo>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Cache components
		this.itemObjs = this.GetNode<Spatial>(this.itemObjsPath);
		this.invButtons = this.GetNode<Control>(this.invButtonsPath);
		this.exam = this.GetNode<ItemExaminer>(this.examPath);
	}

	// Adds an item to the inventory.
	public void AddItem(ItemInfo itemInfo) {
		// Add item to items list
		this.items.Add(itemInfo);

		// Instantiate the item and add to inventory
		PackedScene scn = GD.Load<PackedScene>(ITEMS_PATH + itemInfo.ItemID + TSCN_SUFFIX);
		InteractiveItem item = (InteractiveItem)scn.Instance();
		item.Translate(Vector3.Right * this.itemSeparation * this.itemObjs.GetChildCount());
		item.Scale = Vector3.One * this.itemScale;
		this.itemObjs.AddChild(item);

		// Add a new InvButton to the UI
		Button invButton = new Button();
		invButton.Flat = true;
		invButton.RectMinSize = new Vector2(100, 0);
		invButton.Connect(Signals.PRESSED, this, nameof(_on_InventoryButton_Pressed), new Godot.Collections.Array { this.invButtons.GetChildCount() });
		this.invButtons.AddChild(invButton);
	}

	// Called when inventory button is pressed.
	public void _on_InventoryButton_Pressed(int index) {
		this.exam.SetItem(this.items[index]);
	}
}
