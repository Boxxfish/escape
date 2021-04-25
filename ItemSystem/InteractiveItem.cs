using Godot;
using System;

public class InteractiveItem : StaticBody
{
	public float SELECTED_TINT = 1.1f;

	[Export]
	public NodePath itemSelectorPath;
	[Export]
	public NodePath visualsPath;
	[Export]
	public NodePath interactivesPath;
	[Export]
	public string itemID;
	[Export]
	public bool canPickUp;
	public Inventory Inventory { get; set; }
	public ItemExaminer Examiner { get; set; }

	private ItemSelector itemSelector;
	private Spatial visuals;
	private Spatial interactives;
	private ItemInfo itemInfo;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Cache components
		if (this.itemSelectorPath != null)
			this.itemSelector = this.GetNode<ItemSelector>(this.itemSelectorPath);
		this.visuals = this.GetNode<Spatial>(this.visualsPath);
		this.interactives = this.GetNode<Spatial>(this.interactivesPath);

		// Make materials unique
		this.MakeMaterialsUnique(this.visuals);

		// Register item with selector if not null.
		// Also remove interactions.
		if (this.itemSelectorPath != null) {
			this.itemSelector.RegisterItem(this);
			this.RemoveInteractions();
		}

		// Initialize itemInfo
		this.itemInfo = new ItemInfo();
		this.itemInfo.ItemID = this.itemID;
	}

	// Sets the camera.
	public void SetCam(Camera cam) {
		foreach (ItemInteraction interaction in this.interactives.GetChildren()) {
			interaction.SetCam(cam);
		}
	}

	// Removes the interactions on this object.
	public void RemoveInteractions() {
		this.RemoveChild(this.interactives);
	}

	// Called when cursor enters the item.
	public void _on_InteractiveItem_Enter() {
		SetChildrenTint(this.visuals, SELECTED_TINT);
	}

	// Called when cursor leaves item.
	public void _on_InteractiveItem_Leave() {
		SetChildrenTint(this.visuals, 1.0f / SELECTED_TINT);
	}

	// Recursively sets the tint of all children.
	private void SetChildrenTint(Node node, float tint) {
		if (node is MeshInstance meshNode) {
			SpatialMaterial mat = (SpatialMaterial)meshNode.MaterialOverride;
			mat.AlbedoColor *= tint;
		}

		foreach (Node child in node.GetChildren())
			SetChildrenTint(child, tint);
	}

	// Recursively makes materials unique.
	// If no material exists, set a blank one.
	private void MakeMaterialsUnique(Node node) {
		if (node is MeshInstance meshNode) {
			SpatialMaterial mat = (SpatialMaterial)meshNode.GetActiveMaterial(0);
			if (mat == null)
				meshNode.MaterialOverride = new SpatialMaterial();
			else
				meshNode.MaterialOverride = (SpatialMaterial)mat.Duplicate();
		}

		foreach (Node child in node.GetChildren())
			MakeMaterialsUnique(child);
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
