using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
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
