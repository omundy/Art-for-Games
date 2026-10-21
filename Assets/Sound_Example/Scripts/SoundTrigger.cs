using UnityEngine;

// Sounds from https://sfxr.me/
public class SoundTrigger : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;

    void OnValidate()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
