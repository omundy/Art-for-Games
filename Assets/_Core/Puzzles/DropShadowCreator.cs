using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Attach this to a sprite to add a drop shadow as a child object
 */

public class DropShadowCreator : MonoBehaviour
{
    public GameObject shadowPrefab;
    public DropShadow shadowInstanceScript;

    private void Awake()
    {
        CreateShadow();
    }

    private void Start() { /* to disable in inspector */ }

    void CreateShadow()
    {
        // method 1: create a new GameObject and SpriteRenderer
        //GameObject shadowInstance = new GameObject("Shadow");
        //SpriteRenderer shadowSpriteRenderer = shadowInstance.AddComponent<SpriteRenderer>();

        // method 2: create from prefab - much easier - see DropShadow script on prefab for more details
        GameObject shadowInstance = Instantiate(shadowPrefab, transform.position, Quaternion.identity);


        // set parent
        shadowInstance.transform.SetParent(gameObject.transform);
        // reference to DropShadow script
        shadowInstanceScript = shadowInstance.GetComponent<DropShadow>();
        // initialize the shadow
        shadowInstanceScript.Init(gameObject);

    }
}
