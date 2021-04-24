using Godot;
using System;
using System.Text;

public class TitleScreen : Control
{
	public const string HOST_URL = "http://127.0.0.1:5000";
	public const string GEN_PUZZLES_URL = "genpuzzles";
	public const string GAME_PATH = "res://Scenes/MainGame/main_game.tscn";
	public const string MULTI_CHOOSE_PATH = "res://Scenes/MultiChooseScreen/multi_choose_screen.tscn";

	[Export]
	public NodePath httpPath;
	[Export]
	public NodePath soloButtonPath;
	[Export]
	public NodePath multiButtonPath;

	private HTTPRequest http;
	private Button soloButton;
	private Button multiButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		this.http = this.GetNode<HTTPRequest>(this.httpPath);
		this.soloButton = this.GetNode<Button>(this.soloButtonPath);
		this.multiButton = this.GetNode<Button>(this.multiButtonPath);

		this.http.Connect(Signals.REQUEST_COMPLETED, this, nameof(_on_Http_RequestCompleted));
		this.soloButton.Connect(Signals.PRESSED, this, nameof(this._on_SoloButton_Pressed));
		this.multiButton.Connect(Signals.PRESSED, this, nameof(this._on_MultiButton_Pressed));
	}

	public void _on_SoloButton_Pressed() {
		if (http.Request(HOST_URL + "/" + GEN_PUZZLES_URL) != (int)Error.Ok)
			GD.PrintErr("Error sending request.");
	}
	public void _on_MultiButton_Pressed() {
		this.GetTree().ChangeSceneTo(GD.Load<PackedScene>(MULTI_CHOOSE_PATH));
	}

	public void _on_Http_RequestCompleted(int result, int response_code, string[] headers, byte[] body) {
		if (result == (int)Error.Ok) {
			Godot.Collections.Dictionary json = (Godot.Collections.Dictionary)JSON.Parse(Encoding.UTF8.GetString(body)).Result;
			if (json["status"].Equals("ok")) {
				// Load solo room
				AppState.items = (Godot.Collections.Dictionary)json["items"];
				AppState.rootItem = (string)json["root_name"];
				this.GetTree().ChangeSceneTo(GD.Load<PackedScene>(GAME_PATH));
			}
		}
	}
}
