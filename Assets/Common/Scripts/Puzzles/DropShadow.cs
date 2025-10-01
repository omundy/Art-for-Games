using UnityEngine;

/**
 *  DropShadow - Attached to prefab / new instances of drop shadow
 *  Reference
 *  https://medium.com/@kunaltandon.kt/creating-drop-shadows-for-sprites-in-unity-6415d2b2b9cb
 */

[RequireComponent(typeof(SpriteRenderer))]
public class DropShadow : MonoBehaviour
{
    GameObject parentGameObject;
    SpriteRenderer parentSpriteRenderer;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    public Material shadowMaterial;
    Vector2 offset;
    public Vector2 offsetDown = new Vector2(0.01f, -0.01f);
    public Vector2 offsetUp = new Vector2(0.1f, -0.1f);
    Vector2 scale;
    public float scaleDown = 1.01f;
    public float scaleUp = 1.01f;
    public Color colorDown = new Color(0, 0, 0, .4f);
    public Color colorUp = new Color(0, 0, 0, .3f);

    private void Start() { /* to disable in inspector */ }

    public void Init(GameObject _parent)
    {
        // set parent, name, references
        parentGameObject = _parent;
        name = "Shadow-" + parentGameObject.name;
        spriteRenderer = GetComponent<SpriteRenderer>();
        parentSpriteRenderer = parentGameObject.GetComponent<SpriteRenderer>();

        // set the shadow sprite to the parent sprite
        spriteRenderer.sprite = parentSpriteRenderer.sprite;
        // set shadow material
        spriteRenderer.material = shadowMaterial;

        // call just once to display in default (down)
        DisplayUp(false);
    }

    /**
     *  Change display of shadow - called primarily from parent
     */
    public void DisplayUp(bool up = false)
    {
        if (up)
        {
            scale = Vector2.one * scaleUp;
            offset = offsetUp;
            spriteRenderer.color = colorUp;
        }
        else
        {
            scale = Vector2.one * scaleDown;
            offset = offsetDown;
            spriteRenderer.color = colorDown;
        }

        // SORTING LAYER

        // update the sorting layer and order so it always stays right behind parent
        spriteRenderer.sortingLayerName = parentSpriteRenderer.sortingLayerName;
        spriteRenderer.sortingOrder = parentSpriteRenderer.sortingOrder - 1;

        // DISPLAY WITH ROTATION THAT MATCHES PARENT
        // I tried both non-parented and parented instances.
        // parented are easier to manage but the offset isn't perfect (it
        // doesn't change as you rotate the object). I fixed this by just
        // increasing / decreasing the size of the shadow a bit.

        // non-parented instances => update the position and rotation of the sprite's shadow with moving sprite
        //transform.localScale = parentGameObject.transform.localScale;
        //transform.localPosition = parentGameObject.transform.localPosition + (Vector3)offsetDown;
        //transform.localRotation = parentGameObject.transform.localRotation;

        // parented instances => set shadow transform to defaults
        transform.localScale = scale;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero + (Vector3)offset;
    }


}