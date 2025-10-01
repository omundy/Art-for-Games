using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// DragObject - Drag a GameObject
/// Reference https://www.youtube.com/watch?v=N-HFDCRPcwc
/// 2025 Owen Mundy

public class DragObject : MonoBehaviour
{
    Camera cam;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    private DragObjectManager dragObjectManager;
    private DropShadowCreator dropShadowCreator;

    public Vector2 startDragPosition;

    // start on the ground
    private bool isPickedUp = false;
    public Vector3 mousePos;

    public Vector3 scaleDown;
    public Vector3 scaleUp;
    public Vector3 scaleUpFactor;


    private void Awake()
    {
        if (cam == null) cam = Camera.main;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (dropShadowCreator == null) dropShadowCreator = GetComponent<DropShadowCreator>();
        dragObjectManager = transform.parent.gameObject.GetComponent<DragObjectManager>();
        // set original adjusted scale
        scaleDown = transform.localScale;
        scaleUp = transform.localScale + scaleUpFactor;
    }

    void Update()
    {
        // safety, in case reference is lost while scene switching
        if (cam == null) cam = Camera.main;

        // display the object ...
        DisplayPickedUp();
        DisplayDragOffset();
    }

    /// <summary>
    /// Start dragging
    /// </summary>
    public void PickUp()
    {
        // update mouse and startDrag position
        mousePos = cam.ScreenToWorldPoint(InputManager.Instance.mousePosition);
        startDragPosition = new Vector2(mousePos.x - transform.localPosition.x, mousePos.y - transform.localPosition.y);
        // set isPickedUp status
        isPickedUp = true;

        // notify other GameObjects
        //EventManager.TriggerEvent("OnUpdatePuzzle", gameObject.name);
        // show this one on top

        // move this object to the top of the hierarchy (within the children)
        // SpriteSorter will automatically re-sort sprites
        transform.SetSiblingIndex(0);
    }

    public void PutDown()
    {
        isPickedUp = false;
    }

    /// <summary>
    /// Show that the object has been picked up 
    /// </summary>
    public void DisplayPickedUp()
    {
        if (isPickedUp)
        {
            // update its drop shadow 
            if (dropShadowCreator != null) dropShadowCreator.shadowInstanceScript.DisplayUp(true);
            // update its scale
            transform.localScale = scaleUp;
        }
        else
        {
            if (dropShadowCreator != null) dropShadowCreator.shadowInstanceScript.DisplayUp(false);
            transform.localScale = scaleDown;
        }
    }

    public void DisplayDragOffset()
    {
        // only show offset if picked up
        if (!isPickedUp) return;

        mousePos = cam.ScreenToWorldPoint(InputManager.Instance.mousePosition);
        // offset based on click position
        transform.localPosition = new Vector3(mousePos.x - startDragPosition.x, mousePos.y - startDragPosition.y, transform.localPosition.z);
    }



}
