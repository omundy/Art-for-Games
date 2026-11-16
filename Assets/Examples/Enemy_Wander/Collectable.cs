using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to item (with collider marked as trigger!) to be collected
/// </summary>
public class Collectable : MonoBehaviour
{
    public string itemName;
}

/// <summary>
/// Static class to keep track of items
/// </summary>
public static class Collected_Items
{
    public static List<string> list = new List<string>();
}
