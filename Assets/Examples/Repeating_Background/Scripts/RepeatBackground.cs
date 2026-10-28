using UnityEngine;

public class RepeatBackground : MonoBehaviour
{
    public Vector3 startPos;
    public float fullWidth;
    public float repeatWidth;
    public BoxCollider boxCollider; // this should be on the 2D object that spans the full width to be repeated

    void Start()
    {
        startPos = transform.position;
        fullWidth = boxCollider.size.x;
        repeatWidth = fullWidth / 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < startPos.x - repeatWidth)
        {
            transform.position = startPos;
        }
    }
}
