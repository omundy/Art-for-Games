using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // NEW INPUT SYSTEM

/**
 *  DragObject - Allow a GO to be dragged
 *  Reference https://www.youtube.com/watch?v=N-HFDCRPcwc
 */

public class DragObject : MonoBehaviour
{
    Camera cam;
    Rigidbody2D rb2D;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    private DragObjectManager dragObjectManager;
    private DropShadowCreator dropShadowCreator;

    public Vector2 startDragPosition;

    private float startPosX;
    private float startPosY;
    // start on the ground
    private bool isPickedUp = false;
    public Vector2 velocity = new Vector2(1.75f, 1.1f);
    public Vector3 mousePos;

    public Vector3 scaleDown;
    public Vector3 scaleUp;
    public Vector3 scaleUpFactor;

    public Vector2 mousePosition;
    public InputAction mouseAction;



    private void Awake()
    {
        cam = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2D = GetComponent<Rigidbody2D>();
        dropShadowCreator = GetComponent<DropShadowCreator>();
        dragObjectManager = transform.parent.gameObject.GetComponent<DragObjectManager>();
        // set original adjusted scale
        scaleDown = transform.localScale;
        scaleUp = transform.localScale + scaleUpFactor;



    }

    void Update()
    {
        // safety, it seems the reference is lost in scene switching
        if (cam == null) cam = Camera.main;

        mousePosition = Mouse.current.position.ReadValue();



        // display the object ...
        DisplayPickedUp();
        DisplayDragOffset();
    }



    void OnLeftMousePressed()
    {
        Debug.Log("Left mouse button was pressed this frame!");


    }
    void OnLeftMouseHold()
    {
        Debug.Log("Left mouse button hold - called continously till the button is released");

    }
    void OnLeftMouseReleased()
    {
        Debug.Log("Left mouse button was released this frame!");
    }







    // THESE DON'T WORK - THEY DON'T ALWAYS PICK UP THE OBJECT ON TOP
    //private void OnMouseDown()
    //{
    //    SpriteRenderer selected = SpriteExtensions.ReturnHighestSpriteRendererForDrag<DragObject>(dragObjectManager.interactableLayer);
    //    Debug.Log($"OnMouseDown() spriteRenderer.name={spriteRenderer.name} ?? selected.name={selected.name}");
    //    if (spriteRenderer.name == selected.name)
    //    {
    //        //Debug.Log("OnMouseDown()");
    //        PickUp();
    //    }
    //}
    //private void OnMouseUp()
    //{
    //    //Debug.Log("OnMouseUp()");
    //    PutDown();
    //}



    /**
     *  Start or stop dragging
     */

    public void PickUp()
    {
        // update mouse and startDrag position
        mousePos = cam.ScreenToWorldPoint(mousePosition);
        startDragPosition = new Vector2(mousePos.x - transform.localPosition.x, mousePos.y - transform.localPosition.y);
        // set isPickedUp status
        isPickedUp = true;

        // notify other GameObjects
        //EventManager.TriggerEvent("OnUpdatePuzzle", gameObject.name);
        // show this one on top

        // move this object to the top of the hierarchy (within the children)
        // SpriteSorter will automatically resort sprites
        transform.SetSiblingIndex(0);
    }

    public void PutDown()
    {
        isPickedUp = false;
    }



    /**
     *  Show that the object has been picked up 
     */

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

        mousePos = cam.ScreenToWorldPoint(mousePosition);
        // offset based on click position
        transform.localPosition = new Vector3(mousePos.x - startDragPosition.x, mousePos.y - startDragPosition.y, transform.localPosition.z);
    }









    // OLD ATTEMPT - MARKED FOR DELTEION

    //void FixedUpdate()
    //{
    //    DragPhysics();
    //}

    //void DragPhysics()
    //{
    //    if (isPickedUp)
    //    {
    //        //rb2D.MovePosition(rb2D.position + velocity * Time.fixedDeltaTime);
    //        rb2D.MovePosition(new Vector3(mousePos.x - startPosX, mousePos.y - startPosY, transform.localPosition.z));

    //        //transform.localPosition = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY, transform.localPosition.z);
    //    }
    //}



}
