using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Daylight : MonoBehaviour
{
    // Set these references and day/night values in the Inspector.
    [SerializeField] private Light sun;
    [SerializeField] private Light2D globalLight;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color nightColor = new Color(0.12f, 0.16f, 0.3f, 1f);
    [SerializeField] private Color dayColor = Color.white;
    [SerializeField] private float nightIntensity = 0.1f;
    [SerializeField] private float dayIntensity = 1f;
    [SerializeField] private float dayLengthSeconds = 120f;
    [SerializeField] private float elapsedSeconds;

    private void Update()
    {
        // A non-positive duration cannot produce a valid repeating day cycle.
        if (dayLengthSeconds <= 0f)
        {
            return;
        }

        elapsedSeconds += Time.deltaTime;
        // Keep progress in the [0, 1) range so the cycle repeats after one day.
        float dayProgress = (elapsedSeconds % dayLengthSeconds) / dayLengthSeconds;
        // Smoothly move from night at midnight to full daylight halfway through the cycle.
        float daylight = (1f - Mathf.Cos(dayProgress * 2f * Mathf.PI)) * 0.5f;

        // The light is below the horizon at the start/end and overhead halfway through.
        if (sun != null)
        {
            sun.transform.rotation = Quaternion.Euler(dayProgress * 360f - 90f, 0f, 0f);
        }

        if (globalLight != null)
        {
            // Blend the global light's brightness and tint using the same daylight amount.
            globalLight.intensity = Mathf.Lerp(nightIntensity, dayIntensity, daylight);
            globalLight.color = Color.Lerp(nightColor, dayColor, daylight);
        }
    }
}