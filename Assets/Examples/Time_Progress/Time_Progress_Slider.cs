using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Use a UI Slider as a timer that fills from 0 to duration seconds
/// Call StartTimer() to begin, listen to onFinished to know when time is up
/// </summary>
public class Time_Progress_Slider : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The slider that displays the time")]
    public Slider timeSlider;
    [Tooltip("Optional text that displays the time as m:ss")]
    public TMP_Text timeText;

    [Header("Timer")]
    [Tooltip("Length of the timer in seconds")]
    public float duration = 60f;
    [Tooltip("Seconds since the timer started")]
    public float elapsed = 0f;
    [Tooltip("Is the timer counting?")]
    public bool running = false;

    [Header("Events")]
    [Tooltip("Called once when the timer reaches the end")]
    public UnityEvent onFinished;

    void Start()
    {
        // set the slider range in code so it always matches duration
        timeSlider.minValue = 0;
        timeSlider.maxValue = duration;
        // the player shouldn't be able to drag the timer
        timeSlider.interactable = false;
        UpdateDisplay();
    }

    /// <summary>
    /// Reset to 0 and start counting
    /// </summary>
    public void StartTimer()
    {
        elapsed = 0f;
        running = true;
        UpdateDisplay();
    }

    void Update()
    {
        // only count while the timer is running
        if (!running) return;

        // add the time since the last frame, but don't go past the end
        elapsed = Mathf.Min(elapsed + Time.deltaTime, duration);
        UpdateDisplay();

        // reached the end?
        if (elapsed >= duration)
        {
            running = false;
            Debug.Log("UI_SliderControl: Time's up! " + duration + " seconds have passed.");
            onFinished.Invoke();
        }
    }

    void UpdateDisplay()
    {
        timeSlider.value = elapsed;
        if (timeText != null)
        {
            int seconds = Mathf.FloorToInt(elapsed);
            timeText.text = $"{seconds / 60}:{seconds % 60:00}";
        }
    }
}
