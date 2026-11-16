using TMPro;
using UnityEngine;

public class Trigger_Text : MonoBehaviour
{
    public TMP_Text textAsset;
    public string textToTrigger;

    [Tooltip(
        "If zero, resets after player exits the collider. If greater than zero, resets after that many seconds"
    )]
    public int endTriggerAfter;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && textToTrigger != "" && textAsset != null)
        {
            textAsset.text = textToTrigger;

            if (endTriggerAfter > 0)
                Invoke("HideText", endTriggerAfter);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && textAsset != null)
        {
            if (endTriggerAfter < 1)
                textAsset.text = "";
        }
    }

    void HideText()
    {
        textAsset.text = "";
    }
}
