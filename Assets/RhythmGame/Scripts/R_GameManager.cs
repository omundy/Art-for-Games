using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class R_GameManager : MonoBehaviour
{
    public enum Keys
    {
        Up, Down, Left, Right
    }

    public static R_GameManager instance;
    public AudioSource music;
    public bool started;
    public bool running;
    public int hits;
    public int misses;
    public R_BeatScroller bs;
    public TMP_Text score;

    void Start()
    {
        instance = this;
        hits = 0;
        misses = 0;
    }

    void Update()
    {
        if (!running && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            started = true;
            running = true;
        }
        if (started)
        {
            music.Play();
            started = false;
        }
    }


    public void NoteHit()
    {
        hits++;
        Debug.Log("Hit on time");
        UpdateUI();
    }
    public void NoteMissed()
    {
        misses++;
        Debug.Log("Missed note!");
        UpdateUI();
    }
    public void UpdateUI()
    {
        score.text = $"Hits: {hits}\nMisses: {misses}";
    }
}


public static class R_Colors
{
    public static List<Color> colors = new List<Color>
    {
        new Color(0f, 0f, 1f), // blue
        new Color(1f, 0f, 0f), // red
        new Color(1f, 1f, 0f), // yellow
        new Color(0f, 1f, 0f), // green
    };

    public static Color GetColorFromIndex(int index) => colors[index];
    public static Color GetColorFromKey(string key)
    {
        if (key == "Left") return colors[0];
        else if (key == "Down") return colors[1];
        else if (key == "Up") return colors[2];
        else return colors[3];
    }
}
