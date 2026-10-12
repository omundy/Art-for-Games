using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Control scene by touching this collider
/// </summary>
public class Portal : MonoBehaviour
{

    public string goToScene;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && goToScene != "")
        {
            SceneManager.LoadScene(goToScene);
        }
    }

}
