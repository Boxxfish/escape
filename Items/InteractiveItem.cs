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

	private ItemSelector itemSelector;
	private Spatial visuals;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Cache components
		this.itemSelector = this.GetNode<ItemSelector>(this.itemSelectorPath);
		this.visuals = this.GetNode<Spatial>(this.visualsPath);

		// Register item with selector
		this.itemSelector.RegisterItem(this);
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
		GD.Print("Item Selected.");
	}
}
