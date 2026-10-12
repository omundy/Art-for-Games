using TMPro;
using UnityEngine;

public class TextTrigger : MonoBehaviour
{
    public TMP_Text textAsset;
    public string textToTrigger;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && textToTrigger != "" && textAsset != null)
        {
            textAsset.text = textToTrigger;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && textAsset != null)
        {
            textAsset.text = "";
        }
    }
}
