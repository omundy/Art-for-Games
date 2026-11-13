using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Buttons for testing hidden clue words in the game. Each clue with hidden words, like "3 Down",
/// gets a button that reveals them, then hides them again on the next click.
/// Turn this object off (or delete it) when the buttons aren't needed anymore
/// *** THE BUTTONS ARE DYNAMICALLY CREATED - DON'T EDIT THEM IN THE SCENE ***
/// </summary>
public class Crossword_TestButtons : MonoBehaviour
{
    [Tooltip("The crossword to test")]
    public Crossword crossword;

    [Tooltip("The clues to make buttons for, like \"3 Down\" or \"2 Across\". " +
        "Leave the list empty for a button for every clue that hides words")]
    public List<string> clues = new List<string>();

    [Tooltip("Also add Reveal All and Hide All buttons")]
    public bool allButtons = true;

    [Tooltip("The size of each button, in pixels. They shrink if there are too many to fit")]
    public Vector2 buttonSize = new Vector2(150, 32);

    [Tooltip("The space between the buttons, in pixels")]
    public float spacing = 10;

    // each clue's button label, so it can switch between "Reveal" and "Hide"
    Dictionary<string, TextMeshProUGUI> labels = new Dictionary<string, TextMeshProUGUI>();

    // Start can wait for a frame when it returns IEnumerator
    IEnumerator Start()
    {
        if (crossword == null)
        {
            Debug.LogWarning("Crossword_TestButtons: no Crossword is assigned");
            yield break;
        }

        // wait one frame, so the crossword has read its clues in its own Start()
        yield return null;

        // line the buttons up side by side, centered
        HorizontalLayoutGroup layout = gameObject.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = spacing;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        List<string> clueNames = clues.Count > 0 ? clues : crossword.GetHiddenClueNames();
        foreach (string clueName in clueNames)
        {
            if (clueName.Trim() == "") continue;

            // a copy for the button to remember (the loop's clueName changes each time around)
            string buttonClue = clueName.Trim();
            labels[buttonClue] = MakeButton("", () => Toggle(buttonClue));
        }

        if (allButtons)
        {
            MakeButton("Reveal All", crossword.RevealAllClueWords);
            MakeButton("Hide All", crossword.HideAllClueWords);
        }

        if (clueNames.Count == 0 && !allButtons)
        {
            Debug.LogWarning("Crossword_TestButtons: the clues don't hide any words, so there are no buttons");
        }
    }

    // keep each label right, since words can also be revealed by solving, by other scripts, or by the Inspector
    void Update()
    {
        foreach (KeyValuePair<string, TextMeshProUGUI> label in labels)
        {
            label.Value.text = (crossword.IsClueRevealed(label.Key) ? "Hide " : "Reveal ") + label.Key;
        }
    }

    void Toggle(string clueName)
    {
        if (crossword.IsClueRevealed(clueName))
        {
            // a key named after an answer stays shown while that word is solved
            crossword.HideClue(clueName);
        }
        else
        {
            // send the event for each of the clue's keys, the same way another script in the game would
            foreach (string key in crossword.GetClueKeys(clueName))
            {
                EventManager.TriggerEvent("crosswordReveal", key);
            }
        }
    }

    // make a button that calls onClick, colored like the crossword's squares, and return its label
    TextMeshProUGUI MakeButton(string label, UnityAction onClick)
    {
        GameObject buttonObject = new GameObject("Button", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(transform, false);

        LayoutElement size = buttonObject.GetComponent<LayoutElement>();
        size.preferredWidth = buttonSize.x;
        size.preferredHeight = buttonSize.y;

        Image image = buttonObject.GetComponent<Image>();
        image.color = crossword.style.cellColor;

        // the button darkens the image when it's hovered and pressed
        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        GameObject textObject = new GameObject("Text", typeof(RectTransform));
        textObject.transform.SetParent(buttonObject.transform, false);
        RectTransform textRect = (RectTransform)textObject.transform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(6, 2);
        textRect.offsetMax = new Vector2(-6, -2);

        // use the same font and color as the letters
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.font = crossword.cell.GetComponent<TMP_InputField>().textComponent.font;
        text.color = crossword.style.letterColor;
        text.alignment = TextAlignmentOptions.Center;
        text.enableAutoSizing = true;
        text.fontSizeMin = 8;
        text.fontSizeMax = 18;
        text.raycastTarget = false;
        return text;
    }
}
