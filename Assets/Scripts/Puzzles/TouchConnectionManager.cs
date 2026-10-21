using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  TouchConnectionManager - Get status of all TouchConnection scripts
 */

public class TouchConnectionManager : MonoBehaviour
{
    // array of all TouchConnection components on this GO
    public TouchConnection[] touchConnections;
    // total requirements (for YarnSpinner)
    static public int totalRequirements;
    // whether all requirements have been met
    bool allRequirementsMetLogged;
    public bool allRequirementsMet;
    static public int numRequirementsMet;
    public int numRequirementsMetPublic;
    public Color allRequirementsVisual;



    // begin in start, after puzzle selection
    private void Start()
    {
        touchConnections = GetComponentsInChildren<TouchConnection>();
        totalRequirements = touchConnections.Length;
        StartCoroutine(UpdateTouchStatusLoop());
    }

    IEnumerator UpdateTouchStatusLoop()
    {
        while (true)
        {
            //Debug.Log("UpdateTouchStatusLoop()");
            UpdateTouchStatus();
            yield return new WaitForSeconds(.1f);
        }
    }

    // update status of all TouchConnection components
    void UpdateTouchStatus()
    {
        // use local var until all requirements are counted
        int _numRequirementsMet = 0;
        foreach (TouchConnection tc in touchConnections)
        {
            // if (tc.gameObject.name == this.name) continue; // skip the parent

            if (tc.requirementMet) _numRequirementsMet++;
            //Debug.Log(tc.gameObject.name + ": " + tc.requirementMet);
        }
        // publish
        numRequirementsMet = _numRequirementsMet;
        numRequirementsMetPublic = _numRequirementsMet;
        // reporting
        if (_numRequirementsMet >= touchConnections.Length)
        {
            allRequirementsMet = true;
            allRequirementsVisual = Color.green;
            if (!allRequirementsMetLogged)
            {
                Debug.Log("!!!!!!!!!!!! WIN CONDITION !!!!!!!!!!!!");
                allRequirementsMetLogged = true;
            }
        }
        else
        {
            allRequirementsMet = false;
            allRequirementsVisual = Color.red;
        }
    }




}
