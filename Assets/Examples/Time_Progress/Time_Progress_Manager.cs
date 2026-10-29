using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple game loop with one popup: "Start Game" -> timer runs -> "Game Over" -> play again
/// </summary>
public class Time_Progress_Manager : MonoBehaviour
{
    [Header("Timer")]
    [Tooltip("The timer that ends the game")]
    public Time_Progress_Slider timer;

    [Header("Popup")]
    [Tooltip("The popup panel to show and hide")]
    public GameObject popup;
    [Tooltip("The message text in the popup")]
    public TMP_Text messageText;
    [Tooltip("The button in the popup")]
    public Button button;
    [Tooltip("The text on the button")]
    public TMP_Text buttonText;

    [Header("Freeze")]
    [Tooltip("Scripts to turn off while the popup is showing (e.g. the PlayerController, so it ignores input)")]
    public Behaviour[] disableWhileFrozen;

    [Header("Messages")]
    public string startMessage = "Start Game";
    public string startButton = "Go";
    public string gameOverMessage = "Game Over";
    public string gameOverButton = "Play Again";

    void Start()
    {
        // when the timer finishes, the game is over
        timer.onFinished.AddListener(GameOver);
        ShowPopup(startMessage, startButton, StartGame);
    }

    /// <summary>
    /// Show the popup with a message, and run onClick when the button is pressed
    /// </summary>
    void ShowPopup(string message, string label, UnityAction onClick)
    {
        messageText.text = message;
        buttonText.text = label;
        // remove the previous action so the button only does one thing
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClick);
        popup.SetActive(true);
        Freeze(true);
    }

    void StartGame()
    {
        popup.SetActive(false);
        Freeze(false);
        timer.StartTimer();
    }

    /// <summary>
    /// Stop (or restart) time, physics, animation, and the scripts in disableWhileFrozen
    /// UI buttons still work because they don't use scaled time
    /// </summary>
    void Freeze(bool frozen)
    {
        Time.timeScale = frozen ? 0 : 1;
        foreach (Behaviour b in disableWhileFrozen)
        {
            if (b != null) b.enabled = !frozen;
        }
    }

    void OnDestroy()
    {
        // timeScale isn't reset when a scene loads, so unfreeze before leaving this one
        Time.timeScale = 1;
    }

    void GameOver()
    {
        ShowPopup(gameOverMessage, gameOverButton, PlayAgain);
    }

    void PlayAgain()
    {
        // reload this scene to start everything over (scene must be in File > Build Profiles > Scene List)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
