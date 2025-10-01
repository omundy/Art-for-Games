using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// References
/// UI https://www.youtube.com/watch?v=cZzf1FQQFA0
/// Music https://www.youtube.com/watch?v=PMfhS-kEvc0
/// </summary>

public class R_NoteObject : MonoBehaviour
{
    SpriteRenderer sr;
    public R_GameManager.Keys keyToPress;
    public Color currentColor;
    public bool canBePressed;
    public bool wasHit;
    public bool hasPassed;
    string inputText = "";

    void OnValidate()
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();
        currentColor = R_Colors.GetColorFromKey(keyToPress.ToString());
        sr.color = currentColor;
    }

    void OnEnable()
    {
        EventManager.StartListening("Up", Up);
        EventManager.StartListening("Down", Down);
        EventManager.StartListening("Left", Left);
        EventManager.StartListening("Right", Right);
    }
    void OnDisable()
    {
        EventManager.StopListening("Up", Up);
        EventManager.StopListening("Down", Down);
        EventManager.StopListening("Left", Left);
        EventManager.StopListening("Right", Right);
    }

    void Start()
    {
        Keyboard.current.onTextInput += OnTextInput;
    }

    void Up()
    {
        if (canBePressed && keyToPress == R_GameManager.Keys.Up)
        {
            // Debug.Log("Up");
            Hit();
        }
    }
    void Down()
    {
        if (canBePressed && keyToPress == R_GameManager.Keys.Down)
        {
            // Debug.Log("Down");
            Hit();
        }
    }
    void Left()
    {
        if (canBePressed && keyToPress == R_GameManager.Keys.Left)
        {
            // Debug.Log("Left");
            Hit();
        }
    }
    void Right()
    {
        if (canBePressed && keyToPress == R_GameManager.Keys.Right)
        {
            // Debug.Log("Right");
            Hit();
        }
    }

    private void OnTextInput(char ch)
    {
        inputText += ch;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NoteChecker"))
        {
            canBePressed = true;
            sr.color = Color.white;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NoteChecker"))
        {
            canBePressed = false;
            if (!wasHit)
            {
                R_GameManager.instance.NoteMissed();
                sr.color = Color.Lerp(currentColor, Color.white, .25f);
            }
        }
    }

    void Hit()
    {
        R_GameManager.instance.NoteHit();
        sr.color = Color.Lerp(currentColor, Color.white, .55f);
        wasHit = true;
    }

}
