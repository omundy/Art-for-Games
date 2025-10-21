using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 *  TouchConnection - If two colliders are touching, and if they should
 */

public class TouchConnection : MonoBehaviour
{
    // this would be cool to use one day
    //public enum TouchSetting
    //{
    //    All,    // 0 -> 1, 0 -> 2, 1 -> 2
    //    Linear, // 0 -> 1 -> 2
    //    None,
    //}
    //public TouchSetting touchSetting;



    // colliders to watch
    public Collider2D mainCollider;
    public List<Collider2D> othercolliders;

    // should they touch?
    public List<bool> shouldTouch;

    // should they be hidden on start?
    public bool hideConnectionsOnStart;

    // status / display 
    public List<bool> areTouching;
    [HideInInspector] public bool requirementMet;
    public Color requirementVisual;


    private void Start()
    {
        // begin in start, after puzzle selection
        StartCoroutine(UpdateTouchStatusLoop());

        // hide other renderers
        for (int i = 0; i < othercolliders.Count; i++)
        {
            othercolliders[i].GetComponent<SpriteRenderer>().enabled = !hideConnectionsOnStart;
        }
    }

    IEnumerator UpdateTouchStatusLoop()
    {
        while (true)
        {
            UpdateTouchStatus();
            yield return new WaitForSeconds(.1f);
        }
    }

    // update status of colliders
    void UpdateTouchStatus()
    {
        areTouching = new List<bool>(new bool[othercolliders.Count]);

        for (int i = 0; i < othercolliders.Count; i++)
        {
            // are they touching?
            areTouching[i] = mainCollider.IsTouching(othercolliders[i]);
        }

        for (int i = 0; i < othercolliders.Count; i++)
        {
            // should / are || should not / are not
            if ((shouldTouch[i] && areTouching[i]) || (!shouldTouch[i] && !areTouching[i]))
            {
                requirementMet = true;
            }
            // should / but not, should not / but are
            else //if ((shouldTouch && !areTouching) || (!shouldTouch && areTouching))
            {
                requirementMet = false;
                break;
            }
        }
        DisplayStatus();
    }


    void DisplayStatus()
    {
        if (requirementMet)
            requirementVisual = Color.green;
        else
            requirementVisual = Color.red;
    }


}
