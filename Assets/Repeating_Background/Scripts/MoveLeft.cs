using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    float speed = 10;
    PlayerController_RepeatBackground playerControllerScript;
    float leftBound = -15;

    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController_RepeatBackground>();
    }

    void Update()
    {
        if (!playerControllerScript.gameOver)
            transform.Translate(Vector3.left * Time.deltaTime * speed);

        if (transform.position.x < leftBound && gameObject.CompareTag("Obstacle"))
            Destroy(gameObject);
    }
}
