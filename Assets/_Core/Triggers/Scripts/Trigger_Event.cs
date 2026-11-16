using UnityEngine;

public class Trigger_Event : MonoBehaviour
{
public string eventName;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && eventName != "")
        {
            EventManager.TriggerEvent(eventName);
        }
    }
}
