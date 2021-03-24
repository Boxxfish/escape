using System;
using System.Collections.Generic;

/// <summary>
/// Stores information about items.
/// Can be used to serialize and deserialize items and their state.
/// </summary>
public class ItemInfo {
    public string ItemID { get; set; }
    public Dictionary<string, string> Values { get; } = new Dictionary<string, string>();

    public ItemInfo() {

    }
}
