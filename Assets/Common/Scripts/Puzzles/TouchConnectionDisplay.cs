using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  TouchConnectionDisplay - Potentially use to show hints to user?
 */

public class TouchConnectionDisplay : MonoBehaviour
{
    LineRenderer lineRenderer;


    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start() { /* to disable in inspector */ }


    void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            //Debug.Log(contact.collider.name + " hit " + contact.otherCollider.name);

            // Visualize the contact point
            Debug.DrawRay(contact.point, contact.normal, Color.white);

            // draw a big ugly green line, not sure where to go with this
            //lineRenderer.SetPosition(0, new Vector3(
            //    contact.collider.transform.position.x,
            //    contact.collider.transform.position.y,
            //    contact.collider.transform.position.z));
            //lineRenderer.SetPosition(1, new Vector3(
            //    contact.otherCollider.transform.position.x,
            //    contact.otherCollider.transform.position.y,
            //    contact.otherCollider.transform.position.z));
        }
    }


}
