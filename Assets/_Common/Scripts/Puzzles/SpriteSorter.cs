using System.Collections;
using UnityEngine;

/**
 *  SpriteSorter - Options for keeping child sprites sorted 
 *  1. Organize child sprites into same sorting layer as parent SpriteRenderer
 *  2. Set sorting order of child sprites by position in hierarchy
 *  
 *  Mainly for running in Editor and ensuring hierarchy position is same as sorting order
 *  just to make building game more intuitive.
 */

[ExecuteAlways]
//[ExecuteInEditMode]
public class SpriteSorter : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    [Header("Defaults")]
    public SortingLayer sortingLayer;
    public int sortingLayerID;
    public string sortingLayerName;
    public int sortingOrder;
    [Tooltip("Set this option to make the first in hierarchy the topmost visually")]
    public bool reverseChildSortOrder;

    [Header("Overrides")]
    public string nameOverride;
    public int orderOverride;

    [Header("Children")]
    public int highestChildSortOrder;
    public SpriteRenderer[] childSpriteRenderers;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // debugging
        //DebugSortingLayers();
    }

#if UNITY_EDITOR
    void OnEnable()
    {
        SaveAndSort();
    }
#endif

    void OnTransformChildrenChanged()
    {
        //Debug.Log("SpriteSorter.OnTransformChildrenChanged()");
        SaveAndSort();
    }

    public void SaveAndSort()
    {
        // update sorting layer / order
        SaveDefaultChildSortingLayer();
        SaveDefaultChildSortingOrder();
        SortChildSprites();
    }



    /**
     *  Save default order with inspector settings
     */

    // SAVE NAME
    public void SaveDefaultChildSortingLayer()
    {
        // if an override is set
        if (nameOverride != null && nameOverride != "")
        {
            sortingLayerName = nameOverride;
            sortingLayerID = SortingLayer.NameToID(nameOverride);
        }
        else if (spriteRenderer != null)
        {
            // set name to same as parent
            sortingLayerName = spriteRenderer.sortingLayerName;
            sortingLayerID = sortingLayer.id;
        }
    }
    // SAVE ORDER
    public void SaveDefaultChildSortingOrder()
    {
        // if an override is set
        if (orderOverride != 0)
        {
            sortingOrder = orderOverride;
        }
        else if (spriteRenderer != null)
        {
            // set order to same as parent
            sortingOrder = spriteRenderer.sortingOrder;
        }
    }




    /**
     *  Update child sprites - This was previously a coroutine, unsure if it needs to be
     */

    public void SortChildSprites(float delay = 0.5f)
    {
        //while (true)
        //{
        //Debug.Log("SpriteSorter.SortChildSprites()");

        // highest order
        int highest = 0;

        // update array
        childSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        if (reverseChildSortOrder)
            System.Array.Reverse(childSpriteRenderers);

        int index = sortingOrder | 0;
        // update sorting layer
        foreach (SpriteRenderer sr in childSpriteRenderers)
        {
            // don't change parent
            if (sr.name == name) continue;

            sr.sortingLayerID = sortingLayerID;
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = ++index;

            // update while in this loop instead of calling secondary loop
            if (index > highest) highest = index;
        }
        //UpdateHighestSortOrder();

        //yield return new WaitForSeconds(delay);
        //}
    }




    /**
     *  DEBUGGING / NOT IN USE
     */



    /**
     *  Update the highest sorting order (single expression / expression bodied member)
     */
    public void UpdateHighestSortOrder() => highestChildSortOrder = ReturnHighestSortOrder(childSpriteRenderers);

    /**
     *  Return the highest sorting in an array of sprite renderers
     */
    int ReturnHighestSortOrder(SpriteRenderer[] spriteRenderers)
    {
        //Debug.Log("ReturnHighestSortOrder()");

        int result = 0;
        foreach (SpriteRenderer spriteRenderer in childSpriteRenderers)
        {
            if (spriteRenderer != null)
            {
                if (spriteRenderer.sortingOrder > result) result = spriteRenderer.sortingOrder;
            }
        }
        return result;
    }


    void DebugSortingLayers()
    {
        // list layers
        for (var i = 0; i < SortingLayer.layers.Length; i++)
        {
            Debug.Log(SortingLayer.layers[i].name.ToString());
        }
    }

}
