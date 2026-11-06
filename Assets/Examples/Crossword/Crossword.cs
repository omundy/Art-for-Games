using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Draws a crossword from text by copying a row and a cell, then
/// triggers the "crosswordComplete" event when every letter is correct
/// A finished word that's wrong turns red and isn't saved. Tab moves to the next letter
/// *** CROSSWORD IS DYNAMICALLY CREATED - DON'T EDIT SCENE OBJECTS ***
/// </summary>
public class Crossword : MonoBehaviour
{
    [Tooltip("The row to copy (it should contain the cell)")]
    public GameObject row;

    [Tooltip("The cell to copy")]
    public GameObject cell;

    [Tooltip("The answers. Put a space between squares, use _ for black squares, and make every line the same length")]
    [MonospaceTextArea(10)]
    public string grid =
        "_ _ _ P _ M A S K\n" +
        "S P R I T E _ _ _\n" +
        "H _ _ X _ S _ _ _\n" +
        "A _ _ E _ H U E _\n" +
        "D _ _ L _ _ _ F _\n" +
        "E _ _ _ _ A _ F _\n" +
        "R E N D E R _ E _\n" +
        "_ _ _ O _ T _ C _\n" +
        "F O N T _ _ _ T _";

    // a smaller example to paste into the grid:
    // public string grid =
    // "_ _ _ H O T _\n" +
    // "_ _ _ A _ _ _\n" +
    // "_ _ I T _ _ _";

    [Tooltip("The gap between cells and around the outer edge. Use a whole number so they match")]
    public float cellSpacing = 6;

    [Tooltip("Show a small number in the first cell of each word")]
    public bool showNumbers = true;

    [Tooltip("The letter color in a finished word that's wrong")]
    public Color wrongColor = Color.red;

    // every cell in the grid
    List<Crossword_Cell> cells = new List<Crossword_Cell>();

    // the same cells, stored by row, so we can look at a cell's neighbors
    List<Crossword_Cell[]> cellRows = new List<Crossword_Cell[]>();

    // every word in the grid, across and down. Each is a list of its cells, in order
    List<List<Crossword_Cell>> words = new List<List<Crossword_Cell>>();

    // the word that Tab is moving through
    List<Crossword_Cell> currentWord = null;

    // only trigger the event once
    bool complete = false;

    // used to fit the cells in the available space
    int rowCount = 0;
    int columnCount = 0;
    float otherHeight = 0;

    void Awake()
    {
        // when this scene is added over another one, let that scene's camera do the work
        // (two cameras would also mean two audio listeners, which Unity warns about)
        if (SceneManager.sceneCount > 1)
        {
            foreach (GameObject rootObject in gameObject.scene.GetRootGameObjects())
            {
                if (rootObject.GetComponent<Camera>() != null)
                {
                    rootObject.SetActive(false);
                }
            }
        }
    }

    void Start()
    {
        // move the cell out of the row so copies of the row start empty,
        // then hide both originals because we only want to see the copies
        cell.transform.SetParent(row.transform.parent, false);
        cell.SetActive(false);
        row.SetActive(false);

        SetupLayout();

        string[] lines = grid.Split('\n');
        rowCount = lines.Length;
        for (int r = 0; r < lines.Length; r++)
        {
            GameObject newRow = Instantiate(row, row.transform.parent);
            newRow.name = "Row (" + (r + 1) + ")";
            newRow.SetActive(true);

            // split on spaces, and ignore any extras
            string[] letters = lines[r].Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            columnCount = Mathf.Max(columnCount, letters.Length);
            cellRows.Add(new Crossword_Cell[letters.Length]);
            for (int c = 0; c < letters.Length; c++)
            {
                GameObject newCell = Instantiate(cell, newRow.transform);
                newCell.name = "Cell (" + (r + 1) + ", " + (c + 1) + ")";
                newCell.SetActive(true);

                string letter = letters[c];
                Crossword_Cell crosswordCell = newCell.GetComponent<Crossword_Cell>();
                crosswordCell.Setup(letter == "_" ? "" : letter.Trim().ToUpper(), this);
                cells.Add(crosswordCell);
                cellRows[r][c] = crosswordCell;
            }
        }

        FindWords();

        if (showNumbers)
        {
            AddNumbers();
        }

        ResizeCells();

        LoadLetters();
    }

    // check for the key in Update because a key press only lasts for one frame
    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            MoveToNextLetter();
        }
    }

    // find each run of two or more letters, first across and then down
    void FindWords()
    {
        List<Crossword_Cell> word = new List<Crossword_Cell>();

        for (int r = 0; r < rowCount; r++)
        {
            for (int c = 0; c < columnCount; c++)
            {
                if (HasLetter(r, c)) word.Add(cellRows[r][c]);
                else EndWord(word);
            }
            EndWord(word);
        }

        for (int c = 0; c < columnCount; c++)
        {
            for (int r = 0; r < rowCount; r++)
            {
                if (HasLetter(r, c)) word.Add(cellRows[r][c]);
                else EndWord(word);
            }
            EndWord(word);
        }
    }

    // keep the letters so far if they make a word, then start again
    void EndWord(List<Crossword_Cell> word)
    {
        if (word.Count > 1)
        {
            words.Add(new List<Crossword_Cell>(word));
        }
        word.Clear();
    }

    // go to the next letter in the word, or to the next word after the last letter
    void MoveToNextLetter()
    {
        if (EventSystem.current == null) return;

        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return;

        Crossword_Cell selectedCell = selected.GetComponent<Crossword_Cell>();
        if (selectedCell == null) return;

        // stay in the same word at a crossing, otherwise use the first word with this cell
        if (currentWord == null || !currentWord.Contains(selectedCell))
        {
            currentWord = words.Find(word => word.Contains(selectedCell));
            if (currentWord == null) return;
        }

        int next = currentWord.IndexOf(selectedCell) + 1;
        if (next >= currentWord.Count)
        {
            int nextWord = (words.IndexOf(currentWord) + 1) % words.Count;
            currentWord = words[nextWord];
            next = 0;
        }
        currentWord[next].Focus();
    }

    // number each cell that starts a word, going left to right, then top to bottom
    void AddNumbers()
    {
        int number = 1;
        for (int r = 0; r < cellRows.Count; r++)
        {
            for (int c = 0; c < cellRows[r].Length; c++)
            {
                if (!HasLetter(r, c)) continue;

                // a word starts here if there's no letter before this cell, and there is one after it
                bool startsAcross = !HasLetter(r, c - 1) && HasLetter(r, c + 1);
                bool startsDown = !HasLetter(r - 1, c) && HasLetter(r + 1, c);

                // a cell that starts both an across and a down word only gets one number
                if (startsAcross || startsDown)
                {
                    cellRows[r][c].SetNumber(number);
                    number++;
                }
            }
        }
    }

    // is there a letter at this position? (positions outside the grid don't have one)
    bool HasLetter(int r, int c)
    {
        if (r < 0 || r >= cellRows.Count) return false;
        if (c < 0 || c >= cellRows[r].Length) return false;
        return cellRows[r][c].answer != "";
    }

    // space the rows and cells evenly, and center them instead of stretching them
    void SetupLayout()
    {
        // Grid holds the rows. Its padding makes the outer margin match the gaps between cells
        VerticalLayoutGroup gridLayout = row.transform.parent.GetComponent<VerticalLayoutGroup>();
        int margin = Mathf.RoundToInt(cellSpacing);
        gridLayout.padding = new RectOffset(margin, margin, margin, margin);
        gridLayout.spacing = cellSpacing;
        gridLayout.childControlWidth = true;
        gridLayout.childControlHeight = true;
        gridLayout.childForceExpandWidth = false;
        gridLayout.childForceExpandHeight = false;

        // this object holds the header and Grid. Grid shrinks to fit its rows, and stays centered
        VerticalLayoutGroup crosswordLayout = GetComponent<VerticalLayoutGroup>();
        crosswordLayout.spacing = cellSpacing;
        crosswordLayout.childForceExpandWidth = false;
        crosswordLayout.childForceExpandHeight = false;
        crosswordLayout.childAlignment = TextAnchor.UpperCenter;

        HorizontalLayoutGroup rowLayout = row.GetComponent<HorizontalLayoutGroup>();
        rowLayout.spacing = cellSpacing;
        rowLayout.childForceExpandWidth = false;
        rowLayout.childForceExpandHeight = false;
        rowLayout.childAlignment = TextAnchor.MiddleCenter;

        // anything besides Grid (the header) needs its own space
        otherHeight = 0;
        foreach (RectTransform child in transform)
        {
            if (child != row.transform.parent && child.gameObject.activeSelf)
            {
                otherHeight += LayoutUtility.GetPreferredHeight(child) + cellSpacing;
            }
        }
    }

    // find the largest square that lets the whole grid fit
    void ResizeCells()
    {
        // measure this object, not Grid, because Grid's size depends on the cells
        RectTransform area = (RectTransform)transform;

        // there's one more gap than cells, because of the margin on the outer edges
        float width = (area.rect.width - cellSpacing * (columnCount + 1)) / columnCount;
        float height = (area.rect.height - otherHeight - cellSpacing * (rowCount + 1)) / rowCount;
        float size = Mathf.Min(width, height);

        foreach (Crossword_Cell crosswordCell in cells)
        {
            crosswordCell.SetSize(size);
        }
    }

    // Unity calls this when this object changes size, like when the game window is resized
    void OnRectTransformDimensionsChange()
    {
        if (cells.Count > 0)
        {
            ResizeCells();
        }
    }

    public void OnLetterChanged()
    {
        ShowWrongWords();
        SaveLetters();
        CheckIfComplete();
    }

    // does every cell in the word have a letter?
    bool IsFull(List<Crossword_Cell> word)
    {
        return word.TrueForAll(crosswordCell => crosswordCell.GetLetter() != "");
    }

    bool IsCorrect(List<Crossword_Cell> word)
    {
        return word.TrueForAll(crosswordCell => crosswordCell.IsCorrect());
    }

    // turn the letters red in any finished word that's wrong
    void ShowWrongWords()
    {
        foreach (Crossword_Cell crosswordCell in cells)
        {
            crosswordCell.SetWrong(false, wrongColor);
        }

        foreach (List<Crossword_Cell> word in words)
        {
            if (IsFull(word) && !IsCorrect(word))
            {
                foreach (Crossword_Cell crosswordCell in word)
                {
                    crosswordCell.SetWrong(true, wrongColor);
                }
            }
        }
    }

    // don't save the letters of a finished word that's wrong,
    // unless the letter is also part of a crossing word that's correct
    bool ShouldSave(Crossword_Cell crosswordCell)
    {
        bool inWrongWord = false;

        foreach (List<Crossword_Cell> word in words)
        {
            if (word.Contains(crosswordCell) && IsFull(word))
            {
                if (IsCorrect(word)) return true;
                inWrongWord = true;
            }
        }

        return !inWrongWord;
    }

    // PlayerPrefs stays on this computer after the game quits
    void SaveLetters()
    {
        List<string> letters = new List<string>();
        foreach (Crossword_Cell crosswordCell in cells)
        {
            letters.Add(ShouldSave(crosswordCell) ? crosswordCell.GetLetter() : "");
        }

        // save the puzzle too, so we know which puzzle these letters belong to
        PlayerPrefs.SetString("CrosswordGrid", grid);
        PlayerPrefs.SetString("CrosswordLetters", string.Join(",", letters));
        PlayerPrefs.Save();
    }

    void LoadLetters()
    {
        // ignore saved letters from a different puzzle
        if (PlayerPrefs.GetString("CrosswordGrid") != grid) return;

        string[] letters = PlayerPrefs.GetString("CrosswordLetters").Split(',');
        if (letters.Length != cells.Count) return;

        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].SetLetter(letters[i]);
        }

        ShowWrongWords();

        // a saved puzzle that's already solved triggers the event again
        CheckIfComplete();
    }

    // right-click the script's header in the Inspector to run this
    [ContextMenu("Clear Saved Letters")]
    void ClearSavedLetters()
    {
        PlayerPrefs.DeleteKey("CrosswordGrid");
        PlayerPrefs.DeleteKey("CrosswordLetters");
    }

    void CheckIfComplete()
    {
        if (complete) return;

        foreach (Crossword_Cell crosswordCell in cells)
        {
            if (!crosswordCell.IsCorrect()) return;
        }

        complete = true;
        Debug.Log("crosswordComplete");
        EventManager.TriggerEvent("crosswordComplete");
    }
}
