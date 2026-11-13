using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Go to another scene by touching this collider,
/// or (with Use Key on) by pressing a key while inside it
/// </summary>
public class Trigger_Portal : MonoBehaviour
{
    [Tooltip("Scene to go to (must be in File > Build Profiles > Scene List)")]
    public string goToScene;

    [Tooltip("Off: go as soon as the player touches this. On: wait for the key to be pressed while inside")]
    public bool useKey = false;

    [Tooltip("The key that goes to the scene, when Use Key is on")]
    public Key key = Key.E;

    // count colliders because the player can have more than one, each calls enter and exit
    int playerCollidersInside = 0;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerCollidersInside++;

            if (!useKey)
            {
                GoToScene();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerCollidersInside--;
        }
    }

    // check for the key in Update because a key press only lasts for one frame
    void Update()
    {
        if (
            useKey
            && playerCollidersInside > 0
            && Keyboard.current != null
            && Keyboard.current[key].wasPressedThisFrame
        )
        {
            GoToScene();
        }
    }

    void GoToScene()
    {
        if (goToScene != "")
        {
            SceneManager.LoadScene(goToScene);
        }
    }
}
