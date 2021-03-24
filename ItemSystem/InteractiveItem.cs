using Godot;
using System;

public class InteractiveItem : StaticBody
{
	public float NORMAL_SCALE = 1.0f;
	public float SELECTED_SCALE = 1.02f;

	[Export]
	public NodePath itemSelectorPath;
	[Export]
	public NodePath visualsPath;
	[Export]
	public string itemID;
	[Export]
	public bool canPickUp;
	public Inventory Inventory { get; set; }
	public ItemExaminer Examiner { get; set; }

	private ItemSelector itemSelector;
	private Spatial visuals;
	private ItemInfo itemInfo;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Cache components
		if (this.itemSelectorPath != null)
			this.itemSelector = this.GetNode<ItemSelector>(this.itemSelectorPath);
		this.visuals = this.GetNode<Spatial>(this.visualsPath);

		// Register item with selector if not null
		if (this.itemSelectorPath != null)
			this.itemSelector.RegisterItem(this);

		// Initialize itemInfo
		this.itemInfo = new ItemInfo();
		this.itemInfo.ItemID = this.itemID;
	}

	// Called when cursor enters the item.
	public void _on_InteractiveItem_Enter() {
		this.visuals.Scale = Vector3.One * SELECTED_SCALE;
	}

	// Called when cursor leaves item.
	public void _on_InteractiveItem_Leave() {
		this.visuals.Scale = Vector3.One * NORMAL_SCALE;
	}

	// Called when item is selected.
	public void _on_InteractiveItem_Selected() {
		// If this item can be picked up, add it to the inventory
		// and remove it from the scene
		if (this.canPickUp) {
			this.Inventory.AddItem(this.itemInfo);
			this.itemSelector.DeregisterItem(this);
			this.GetParent().RemoveChild(this);
		}
		// Otherwise, let it be examined
		else {
			this.Examiner.SetItem(this.itemInfo);
		}
	}
}
