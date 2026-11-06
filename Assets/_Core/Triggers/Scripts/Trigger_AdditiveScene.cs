using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Add a scene over the current one, or remove it, by pressing a key while inside this collider
/// The scene is also removed when the player leaves the collider
/// Removing a scene loses any changes in it, unless the scene saves them (see Crossword.cs)
/// </summary>
public class Trigger_AdditiveScene : MonoBehaviour
{
    [Tooltip("Scene to add (must be in File > Build Profiles > Scene List)")]
    public string sceneToLoad;

    [Tooltip("The key that adds the scene, or removes it if it's open")]
    public Key key = Key.E;

    // count colliders because the player can have more than one, each calls enter and exit
    int playerCollidersInside = 0;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerCollidersInside++;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerCollidersInside--;

            // the whole player has walked away
            if (playerCollidersInside == 0)
            {
                CloseScene();
            }
        }
    }

    // check for the key in Update because a key press only lasts for one frame
    void Update()
    {
        if (
            playerCollidersInside > 0
            && sceneToLoad != ""
            && Keyboard.current[key].wasPressedThisFrame
        )
        {
            // the same key opens the scene or, if it's already open, closes it
            if (!SceneManager.GetSceneByName(sceneToLoad).isLoaded)
            {
                SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
            }
            else
            {
                CloseScene();
            }
        }
    }

    void CloseScene()
    {
        // nothing to do if it's already closed
        if (SceneManager.GetSceneByName(sceneToLoad).isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneToLoad);
        }
    }
}
