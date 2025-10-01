using UnityEngine;
using UnityEngine.InputSystem;

public class R_NoteChecker : MonoBehaviour
{
    SpriteRenderer sr;
    public R_GameManager.Keys keyToPress;
    public Color currentColor;
    public Color pressedColor;

    void OnValidate()
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();
        currentColor = R_Colors.GetColorFromKey(keyToPress.ToString());
        sr.color = currentColor;
        pressedColor = Color.Lerp(currentColor, Color.white, .55f);
    }

    void Update()
    {
        // get any key
        // InputSystem.onAnyButtonPress.Call( ctrl => {
        //         Debug.Log($"Button {ctrl} on device {ctrl.device} with ID {ctrl.device.deviceId} was pressed");
        // });


        if (keyToPress == R_GameManager.Keys.Left)
        {
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                EventManager.TriggerEvent("Left");
                SetActiveColor();
            }
            else if (Keyboard.current.leftArrowKey.wasReleasedThisFrame)
                SetColor();
        }
        if (keyToPress == R_GameManager.Keys.Down)
        {
            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                EventManager.TriggerEvent("Down");
                SetActiveColor();
            }
            else if (Keyboard.current.downArrowKey.wasReleasedThisFrame)
                SetColor();
        }
        if (keyToPress == R_GameManager.Keys.Up)
        {
            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                EventManager.TriggerEvent("Up");
                SetActiveColor();
            }
            else if (Keyboard.current.upArrowKey.wasReleasedThisFrame)
                SetColor();
        }
        if (keyToPress == R_GameManager.Keys.Right)
        {
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                EventManager.TriggerEvent("Right");
                SetActiveColor();
            }
            else if (Keyboard.current.rightArrowKey.wasReleasedThisFrame)
                SetColor();
        }
    }

    void SetColor()
    {
        sr.color = currentColor;
    }

    void SetActiveColor()
    {
        sr.color = pressedColor;
    }
}
