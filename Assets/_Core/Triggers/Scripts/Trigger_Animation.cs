using UnityEngine;

public class Trigger_Animation : MonoBehaviour
{
    public Animator animator;

    void Awake()
    {
        animator.SetInteger("State", 0);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && animator != null)
        {
            animator.SetInteger("State", 1);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && animator != null)
        {
            animator.SetInteger("State", 3);
        }
    }
}
