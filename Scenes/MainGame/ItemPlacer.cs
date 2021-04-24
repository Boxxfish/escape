using Godot;
using System;
using System.Collections.Generic;

public class ItemPlacer : Spatial
{
	public const string ITEM_PATH = "res://ItemSystem/Items/";

	[Export]
	public NodePath itemSelectorPath;

	private ItemSelector itemSelector;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Cache nodes
		this.itemSelector = this.GetNode<ItemSelector>(this.itemSelectorPath);

		// Load test_puzzles.json
		if (AppState.items == null) {
			File testPuzzlesFile = new File();
			testPuzzlesFile.Open("test_puzzles.json", File.ModeFlags.Read);
			Godot.Collections.Dictionary testPuzzlesDict = (Godot.Collections.Dictionary)JSON.Parse(testPuzzlesFile.GetAsText()).Result;
			AppState.items = (Godot.Collections.Dictionary)testPuzzlesDict["items"];
			AppState.rootItem = (string)testPuzzlesDict["root_name"];
		}

		// Place items
		Queue<Godot.Collections.Dictionary> itemQueue = new Queue<Godot.Collections.Dictionary>();
		itemQueue.Enqueue((Godot.Collections.Dictionary)AppState.items[AppState.rootItem]);
		while (itemQueue.Count > 0) {
			Godot.Collections.Dictionary item = itemQueue.Dequeue();

			// Skip if room node
			if ((string)item["id"] == "room")
				continue;

			// Set up RNG
			Random rng = new Random((int)((float)item["seed"]));

			// Load item
			PackedScene itemScn = GD.Load<PackedScene>(ITEM_PATH + item["id"] + ".tscn");
			if (itemScn != null) {
				InteractiveItem newItem = (InteractiveItem)itemScn.Instance();
				newItem.itemSelectorPath = this.itemSelector.GetPath();
				this.AddChild(newItem);

				// Move item against the wall
				int wallNum = rng.Next(0, 4);
				float wallSize = 10.0f;
				float wallSide = ((float)rng.NextDouble() - 0.5f) * wallSize;
				switch (wallNum) {
					case 0:
						newItem.Translate(Vector3.Forward * wallSize / 2.0f);
						newItem.Translate(Vector3.Left * wallSide);
						break;
					case 1:
						newItem.RotateY(Mathf.Pi);
						newItem.Translate(Vector3.Forward * wallSize / 2.0f);
						newItem.Translate(Vector3.Left * wallSide);
						break;
					case 2:
						newItem.RotateY(Mathf.Pi / 2.0f);
						newItem.Translate(Vector3.Forward * wallSize / 2.0f);
						newItem.Translate(Vector3.Left * wallSide);
						break;
					case 3:
						newItem.RotateY(-Mathf.Pi / 2.0f);
						newItem.Translate(Vector3.Forward * wallSize / 2.0f);
						newItem.Translate(Vector3.Left * wallSide);
						break;
				}
			}

			// Propegate progenitor nodes
			if (item["from"] != null)
				itemQueue.Enqueue((Godot.Collections.Dictionary)AppState.items[item["from"]]);
			foreach (string prereq in (Godot.Collections.Array)item["prereqs"]) {
				if (prereq != null)
					itemQueue.Enqueue((Godot.Collections.Dictionary)AppState.items[prereq]);
			}
		}
	}
}
