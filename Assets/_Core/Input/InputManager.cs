using System;
using UnityEngine;
using UnityEngine.InputSystem; // NEW INPUT SYSTEM

/// Simple way to use new Input System
// Reference https://onewheelstudio.com/blog/2023/7/4/easy-mode-unitys-new-input-system
/// 2025 Owen Mundy

public class InputManager : MonoBehaviour
{
    public Vector2 mousePosition;
    public MouseButton mouseLeft;
    public MouseButton mouseRight;
    // singleton
    public static InputManager Instance { get; private set; }

    void Awake()
    {
        // singleton
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Mouse.current.position.ReadValue();
        mouseLeft = new MouseButton();
        mouseRight = new MouseButton();

        // LEFT
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            EventManager.TriggerEvent("MouseButtonLeft_Down");
            mouseLeft.down = true;
        }
        if (Mouse.current.leftButton.isPressed)
        {
            EventManager.TriggerEvent("MouseButtonLeft_Hold");
            mouseLeft.hold = true;
        }
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            EventManager.TriggerEvent("MouseButtonLeft_Up");
            mouseLeft.up = true;
        }

        // RIGHT
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            EventManager.TriggerEvent("MouseButtonRight_Down");
            mouseRight.down = true;
        }
        if (Mouse.current.rightButton.isPressed)
        {
            EventManager.TriggerEvent("MouseButtonRight_Hold");
            mouseRight.hold = true;
        }
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            EventManager.TriggerEvent("MouseButtonRight_Up");
            mouseRight.up = true;
        }

        // ...
    }
}

[Serializable]
public struct MouseButton
{
    public MouseButton(bool down = false, bool hold = false, bool up = false)
    {
        this.down = down;
        this.hold = hold;
        this.up = up;
    }
    public bool down; // a.k.a "started" or "wasPressedThisFrame"
    public bool hold; // a.k.a "performed" or "isPressed"
    public bool up;   // a.k.a "canceled" or "wasReleasedThisFrame"
}