using UnityEngine;

public class R_BeatScroller : MonoBehaviour
{
    public Vector2 playerInput;
    // music tempo has to match tempo of music you are using
    public float beatTempo;

    void Start()
    {
        beatTempo = beatTempo / 60f; // how fast beats should move per second
    }

    void Update()
    {
        if (R_GameManager.instance.running)
        {
            transform.position -= new Vector3(0f, beatTempo * Time.deltaTime, 0f);
        }
    }
}
