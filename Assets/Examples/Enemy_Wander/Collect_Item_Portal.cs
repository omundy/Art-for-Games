using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Attach to portal (with collider marked as trigger!) that only works if item collected
/// </summary>
public class CollectedItem_Portal : MonoBehaviour
{
    [Tooltip("Scene to go to (must be in File > Build Profiles > Scene List)")]
    public string goToScene;

    [Tooltip("The name of the item required (must match!!)")]
    public string itemRequired;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (itemRequired != "" && Collected_Items.list != null && Collected_Items.list.Contains(itemRequired))
                GoToScene();
        }
    }

    void GoToScene()
    {
        if (goToScene != "")
            SceneManager.LoadScene(goToScene);
    }
}
