using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Place on Player to collect items containing Collectable script
/// </summary>
public class Player_Collect_Item : MonoBehaviour
{
    public List<string> collected = new List<string>();

    void OnTriggerEnter2D(Collider2D collision)
    {
        Collectable collectable = collision.gameObject.GetComponent<Collectable>();
        if (collectable != null)
        {
            // if has collectable script
            Collected_Items.list.Add(collectable.itemName);
            collected = Collected_Items.list;
            Destroy(collision.gameObject);
        }
    }
}
