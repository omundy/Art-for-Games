using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// One square in the crossword: either a letter to fill in, or a black square
/// </summary>
public class Crossword_Cell : MonoBehaviour
{
    [Tooltip("The correct letter (empty for a black square)")]
    public string answer = "";

    TMP_InputField input;

    // the letter color when the cell isn't marked as wrong
    Color normalColor;

    // the small number in the corner (only some cells have one)
    TextMeshProUGUI numberText;

    public void Setup(string letter, Crossword crossword)
    {
        answer = letter;
        input = GetComponent<TMP_InputField>();
        normalColor = input.textComponent.color;

        // the placeholder would show a letter in every empty square
        input.placeholder.GetComponent<TMP_Text>().text = "";

        if (answer == "")
        {
            // turn off the input so it can't be clicked. (setting interactable to false
            // would also apply the input's "disabled" color, making the black see-through)
            GetComponent<Image>().color = Color.black;
            input.enabled = false;
            return;
        }

        // one capital letter per square
        input.characterLimit = 1;
        input.onValidateInput = (text, index, character) =>
            char.IsLetter(character) ? char.ToUpper(character) : '\0';

        // check the whole puzzle each time this letter changes
        input.onValueChanged.AddListener(text => crossword.OnLetterChanged());
    }

    // add a small number in the top left corner
    public void SetNumber(int number)
    {
        GameObject numberObject = new GameObject("Number", typeof(RectTransform));
        numberObject.transform.SetParent(transform, false);

        // fill the cell, leaving a small margin
        RectTransform rect = (RectTransform)numberObject.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(4, 2);
        rect.offsetMax = new Vector2(-4, -2);

        numberText = numberObject.AddComponent<TextMeshProUGUI>();
        numberText.text = number.ToString();
        numberText.alignment = TextAlignmentOptions.TopLeft;
        // match the letters
        numberText.font = input.textComponent.font;
        numberText.color = input.textComponent.color;
        // let clicks pass through to the input
        numberText.raycastTarget = false;
    }

    // use one number for the width and height, so the cell is always square
    public void SetSize(float size)
    {
        LayoutElement layout = GetComponent<LayoutElement>();
        layout.preferredWidth = size;
        layout.preferredHeight = size;
        // don't stretch to fill any extra space in the row
        layout.flexibleWidth = 0;
        layout.flexibleHeight = 0;

        // keep the number in proportion to the cell
        if (numberText != null)
        {
            numberText.fontSize = size * 0.25f;
        }
    }

    // show whether this cell is part of a wrong word
    public void SetWrong(bool wrong, Color wrongColor)
    {
        input.textComponent.color = wrong ? wrongColor : normalColor;
    }

    // move the cursor to this cell, so the player can type in it
    public void Focus()
    {
        input.Select();
        input.ActivateInputField();
    }

    public string GetLetter()
    {
        return input.text;
    }

    // restore a saved letter without running the "letter changed" code
    public void SetLetter(string letter)
    {
        input.SetTextWithoutNotify(letter);
    }

    // black squares are always correct, their text and answer are both empty
    public bool IsCorrect()
    {
        return input.text.ToUpper() == answer;
    }
}
