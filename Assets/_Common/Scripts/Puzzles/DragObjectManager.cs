using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomExtensions;

/**
 *  DragObjectManager - Events, Properties, & methods for all DragObjects && RotateObjects
 */

public class DragObjectManager : MonoBehaviour
{
    //[Tooltip("The sprite renderer currently selected")]
    //public SpriteRenderer spriteRenderer;

    [Tooltip("Selected drag object")]
    public DragObject dragObjSelected;

    [Tooltip("Selected rotate object")]
    public RotateObject rotateObjSelected;

    [Tooltip("The layers that can be interacted")]
    public LayerMask interactableLayer;

    // adding this here, does it make allocation a problem still?
    public RaycastHit2D[] hits;


    // listeners
    void OnEnable()
    {
        EventManager.StartListening("MouseButtonLeft_Down", MouseButtonLeft_Down);
        EventManager.StartListening("MouseButtonLeft_Up", MouseButtonLeft_Up);
        EventManager.StartListening("MouseButtonRight_Down", MouseButtonRight_Down);
        EventManager.StartListening("MouseButtonRight_Up", MouseButtonRight_Up);
    }
    void OnDisable()
    {
        EventManager.StopListening("MouseButtonLeft_Down", MouseButtonLeft_Down);
        EventManager.StopListening("MouseButtonLeft_Up", MouseButtonLeft_Up);
        EventManager.StopListening("MouseButtonRight_Down", MouseButtonRight_Down);
        EventManager.StopListening("MouseButtonRight_Up", MouseButtonRight_Up);
    }

    // SELECTION

    void MouseButtonLeft_Down() => PickUpDragObject(
        SpriteExtensions.ReturnHighestSpriteRendererForDrag<DragObject>(interactableLayer));
    void MouseButtonLeft_Up() => PutDownDragObject();

    // ROTATION

    void MouseButtonRight_Down() => StartRotateDragObject(
        SpriteExtensions.ReturnHighestSpriteRendererForDrag<DragObject>(interactableLayer));
    void MouseButtonRight_Up() => StopRotateDragObject();








    /// <summary>Pick up a drag object</summary>
    /// <param name="sr"></param>
    public void PickUpDragObject(SpriteRenderer sr)
    {
        if (sr == null) return;
        dragObjSelected = sr.GetComponent<DragObject>();
        dragObjSelected.PickUp();
    }

    /// <summary>Put down the selected drag object</summary>
    public void PutDownDragObject()
    {
        if (!dragObjSelected) return;
        dragObjSelected.PutDown();
        dragObjSelected = null;
    }

    /// <summary>Start rotating an object</summary>
    /// <param name="sr"></param>
    void StartRotateDragObject(SpriteRenderer sr)
    {
        if (sr == null) return;
        rotateObjSelected = sr.GetComponent<RotateObject>();
        rotateObjSelected.OnStartRotate();
    }

    /// <summary>Stop rotating an object</summary>
    void StopRotateDragObject()
    {
        if (!rotateObjSelected) return;
        rotateObjSelected.OnStopRotate();
        rotateObjSelected = null;
    }






}
