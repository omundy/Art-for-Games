using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Draws a crossword from text by copying a row and a cell, then
/// triggers the "crosswordComplete" event when every letter is correct
/// A finished word that's wrong turns red and isn't saved. Tab moves to the next letter
/// Clues are listed beside the grid: Across on the left, Down on the right. The clue for the
/// word being typed in is highlighted, solved clues fade, and clue words can stay hidden until revealed
/// The answers and clues come from a Crossword_Puzzle asset, and the look from a Crossword_Style asset
/// *** CROSSWORD IS DYNAMICALLY CREATED - DON'T EDIT SCENE OBJECTS ***
/// </summary>
public class Crossword : MonoBehaviour
{
    [Tooltip("The row to copy (it should contain the cell)")]
    public GameObject row;

    [Tooltip("The cell to copy")]
    public GameObject cell;

    [Tooltip("The answers and clues. Make one with Assets > Create > Crossword > Puzzle")]
    public Crossword_Puzzle puzzle;

    [Tooltip("The colors, spacing and clue layout. Make one with Assets > Create > Crossword > Style")]
    public Crossword_Style style;

    [Tooltip("Optional. Leave empty and a panel is made to the left of the grid. " +
        "Your own text isn't moved or recolored, only filled in")]
    public TextMeshProUGUI acrossText;

    [Tooltip("Optional. Leave empty and a panel is made to the right of the grid. " +
        "Your own text isn't moved or recolored, only filled in")]
    public TextMeshProUGUI downText;

    // every cell in the grid
    List<Crossword_Cell> cells = new List<Crossword_Cell>();

    // the same cells, stored by row, so we can look at a cell's neighbors
    List<Crossword_Cell[]> cellRows = new List<Crossword_Cell[]>();

    // every word in the grid, across and down. Each is a list of its cells, in order
    List<List<Crossword_Cell>> words = new List<List<Crossword_Cell>>();

    // the same words, split into across and down for the clue lists
    List<List<Crossword_Cell>> acrossWords = new List<List<Crossword_Cell>>();
    List<List<Crossword_Cell>> downWords = new List<List<Crossword_Cell>>();

    // each answer's clue, like "MASK" -> "Hides part of an image"
    Dictionary<string, string> clueByAnswer = new Dictionary<string, string>();

    // clues changed with SetClue(), kept so they aren't lost when the puzzle asset is edited
    Dictionary<string, string> changedClues = new Dictionary<string, string>();

    // the grid the cells were made from, to spot when it's edited during Play mode
    string builtGrid = "";

    // the assets whose changes we're listening to
    Crossword_Puzzle listeningPuzzle;
    Crossword_Style listeningStyle;

    // keys of the hidden clue words that have been revealed (solved answers reveal their own key too)
    HashSet<string> revealedKeys = new HashSet<string>();

    // the panels this script made (empty if acrossText and downText were assigned)
    RectTransform acrossPanel;
    RectTransform downPanel;

    // move the panels after the layout has updated, see LateUpdate()
    bool placeClues = false;

    // a value changed in the Inspector during Play mode, see Update()
    bool settingsChanged = false;

    // the word whose clue is highlighted
    List<Crossword_Cell> highlightedWord = null;

    // finds {key:hidden words} in a clue
    static readonly Regex hiddenPattern = new Regex(@"\{([^{}:]+):([^{}]*)\}");

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
        // without assets, use the default values written in Crossword_Puzzle.cs and Crossword_Style.cs
        if (puzzle == null)
        {
            Debug.LogWarning("Crossword: no Puzzle is assigned, so the default puzzle is used");
            puzzle = ScriptableObject.CreateInstance<Crossword_Puzzle>();
            puzzle.name = "Default";
        }
        if (style == null)
        {
            Debug.LogWarning("Crossword: no Style is assigned, so the default style is used");
            style = ScriptableObject.CreateInstance<Crossword_Style>();
            style.name = "Default";
        }

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

        builtGrid = puzzle.grid;
        string[] lines = puzzle.grid.Split('\n');
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

        // the clues use the numbers, so work them out either way
        AddNumbers(style.showNumbers || style.showClues);

        LoadLetters();

        // the panels are made first, so ResizeCells() can leave room for them
        SetupClues();

        ApplyColors();

        ResizeCells();

        RefreshClues();
    }

    // Unity calls this when a value is changed in the Inspector
    void OnValidate()
    {
        // changing the UI in here makes Unity complain, so wait for Update()
        if (Application.isPlaying) settingsChanged = true;
    }

    // called by the Puzzle and Style assets when they're changed in the Inspector
    void OnAssetChanged()
    {
        settingsChanged = true;
    }

    // listen to the assigned assets (they can be swapped in the Inspector, so check each time)
    void ListenToAssets()
    {
        if (listeningPuzzle != puzzle)
        {
            if (listeningPuzzle != null) listeningPuzzle.changed -= OnAssetChanged;
            if (puzzle != null) puzzle.changed += OnAssetChanged;
            listeningPuzzle = puzzle;
        }
        if (listeningStyle != style)
        {
            if (listeningStyle != null) listeningStyle.changed -= OnAssetChanged;
            if (style != null) style.changed += OnAssetChanged;
            listeningStyle = style;
        }
    }

    void StopListeningToAssets()
    {
        if (listeningPuzzle != null) listeningPuzzle.changed -= OnAssetChanged;
        if (listeningStyle != null) listeningStyle.changed -= OnAssetChanged;
        listeningPuzzle = null;
        listeningStyle = null;
    }

    // update the colors, sizes and clues when the Inspector changes them
    void ApplySettings()
    {
        // a missing asset would break everything below, so wait until one is assigned
        if (puzzle == null || style == null) return;
        ListenToAssets();

        // the cells are only made once, in Start()
        if (puzzle.grid != builtGrid)
        {
            Debug.LogWarning("Crossword: the grid changed. Stop and start Play mode to see it");
            builtGrid = puzzle.grid;
        }

        ReadClues();
        ApplyColors();
        ShowWrongWords();
        ResizeCells();
        RefreshClues();
    }

    // listen for other scripts revealing hidden clue words
    void OnEnable()
    {
        EventManager.StartListening("crosswordReveal", OnRevealEvent);
        ListenToAssets();
    }

    void OnDisable()
    {
        EventManager.StopListening("crosswordReveal", OnRevealEvent);
        StopListeningToAssets();
    }

    void OnRevealEvent(object key)
    {
        RevealClueWords(key.ToString());
    }

    // check for the key in Update because a key press only lasts for one frame
    void Update()
    {
        if (settingsChanged)
        {
            settingsChanged = false;
            ApplySettings();
        }

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            MoveToNextLetter();
        }

        // highlight the clue for the word being typed in, but only rebuild the clues when it changes
        List<Crossword_Cell> word = SelectedWord();
        if (word != highlightedWord)
        {
            highlightedWord = word;
            RefreshClues();
        }
    }

    // LateUpdate runs after every Update, but before the layout is drawn
    void LateUpdate()
    {
        if (placeClues)
        {
            placeClues = false;
            PlaceCluePanels();
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
                else EndWord(word, acrossWords);
            }
            EndWord(word, acrossWords);
        }

        for (int c = 0; c < columnCount; c++)
        {
            for (int r = 0; r < rowCount; r++)
            {
                if (HasLetter(r, c)) word.Add(cellRows[r][c]);
                else EndWord(word, downWords);
            }
            EndWord(word, downWords);
        }

        // Tab goes through every across word, then every down word
        words.AddRange(acrossWords);
        words.AddRange(downWords);
    }

    // keep the letters so far if they make a word, then start again
    void EndWord(List<Crossword_Cell> word, List<List<Crossword_Cell>> list)
    {
        if (word.Count > 1)
        {
            list.Add(new List<Crossword_Cell>(word));
        }
        word.Clear();
    }

    // go to the next letter in the word, or to the next word after the last letter
    void MoveToNextLetter()
    {
        Crossword_Cell selectedCell = SelectedCell();
        if (SelectedWord() == null) return;

        int next = currentWord.IndexOf(selectedCell) + 1;
        if (next >= currentWord.Count)
        {
            int nextWord = (words.IndexOf(currentWord) + 1) % words.Count;
            currentWord = words[nextWord];
            next = 0;
        }
        currentWord[next].Focus();
    }

    // the cell the player is typing in, or null if none is selected
    Crossword_Cell SelectedCell()
    {
        if (EventSystem.current == null) return null;

        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return null;

        return selected.GetComponent<Crossword_Cell>();
    }

    // the word the selected cell is in, or null if no cell is selected
    List<Crossword_Cell> SelectedWord()
    {
        Crossword_Cell selectedCell = SelectedCell();
        if (selectedCell == null) return null;

        // stay in the same word at a crossing, otherwise use the first word with this cell
        if (currentWord == null || !currentWord.Contains(selectedCell))
        {
            currentWord = words.Find(word => word.Contains(selectedCell));
        }
        return currentWord;
    }

    // number each cell that starts a word, going left to right, then top to bottom.
    // draw decides whether the numbers are shown in the cells
    void AddNumbers(bool draw)
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
                    cellRows[r][c].number = number;
                    if (draw)
                    {
                        cellRows[r][c].SetNumber(number);
                    }
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
        int margin = Mathf.RoundToInt(style.cellSpacing);
        gridLayout.padding = new RectOffset(margin, margin, margin, margin);
        gridLayout.spacing = style.cellSpacing;
        gridLayout.childControlWidth = true;
        gridLayout.childControlHeight = true;
        gridLayout.childForceExpandWidth = false;
        gridLayout.childForceExpandHeight = false;

        // this object holds the header and Grid. Grid shrinks to fit its rows, and stays centered
        VerticalLayoutGroup crosswordLayout = GetComponent<VerticalLayoutGroup>();
        crosswordLayout.spacing = style.cellSpacing;
        crosswordLayout.childForceExpandWidth = false;
        crosswordLayout.childForceExpandHeight = false;
        crosswordLayout.childAlignment = TextAnchor.UpperCenter;

        HorizontalLayoutGroup rowLayout = row.GetComponent<HorizontalLayoutGroup>();
        rowLayout.spacing = style.cellSpacing;
        rowLayout.childForceExpandWidth = false;
        rowLayout.childForceExpandHeight = false;
        rowLayout.childAlignment = TextAnchor.MiddleCenter;

        // anything besides Grid (the header) needs its own space
        otherHeight = 0;
        foreach (RectTransform child in transform)
        {
            if (child != row.transform.parent && child.gameObject.activeSelf)
            {
                otherHeight += LayoutUtility.GetPreferredHeight(child) + style.cellSpacing;
            }
        }
    }

    // find the largest square that lets the whole grid fit
    void ResizeCells()
    {
        // measure this object, not Grid, because Grid's size depends on the cells
        RectTransform area = (RectTransform)transform;

        // there's one more gap than cells, because of the margin on the outer edges
        float width = (GridWidthLimit() - style.cellSpacing * (columnCount + 1)) / columnCount;
        float height = (area.rect.height - otherHeight - style.cellSpacing * (rowCount + 1)) / rowCount;
        float size = Mathf.Min(width, height);

        foreach (Crossword_Cell crosswordCell in cells)
        {
            crosswordCell.SetSize(size);
        }

        // the grid changed size, so the clue panels need to move
        placeClues = true;
    }

    // how wide the grid can be. With clue panels, it has to leave room for one on each side
    float GridWidthLimit()
    {
        RectTransform area = (RectTransform)transform;
        if (!style.showClues || acrossPanel == null) return area.rect.width;

        // the grid is centered in this object, so measure from its center to the nearer screen edge
        RectTransform screen = (RectTransform)transform.parent;
        float center = screen.InverseTransformPoint(area.TransformPoint(area.rect.center)).x;
        float room = Mathf.Min(center - screen.rect.xMin, screen.rect.xMax - center);

        // take away a panel, the gap and a small margin from each side
        float half = room - style.clueWidth * screen.rect.width - style.clueGap - style.cellSpacing;
        return Mathf.Clamp(half * 2, 1, area.rect.width);
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
        RefreshClues();
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
            crosswordCell.SetWrong(false, style.wrongColor);
        }

        foreach (List<Crossword_Cell> word in words)
        {
            if (IsFull(word) && !IsCorrect(word))
            {
                foreach (Crossword_Cell crosswordCell in word)
                {
                    crosswordCell.SetWrong(true, style.wrongColor);
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

        // save the grid too, so we know these letters still fit it
        PlayerPrefs.SetString(SaveKey("Grid"), puzzle.grid);
        PlayerPrefs.SetString(SaveKey("Letters"), string.Join(",", letters));
        PlayerPrefs.SetString(SaveKey("Revealed"), string.Join("\n", revealedKeys));
        PlayerPrefs.Save();
    }

    void LoadLetters()
    {
        // ignore saved letters if the grid has been changed since
        if (PlayerPrefs.GetString(SaveKey("Grid")) != puzzle.grid) return;

        string revealed = PlayerPrefs.GetString(SaveKey("Revealed"));
        revealedKeys.UnionWith(revealed.Split('\n', System.StringSplitOptions.RemoveEmptyEntries));

        string[] letters = PlayerPrefs.GetString(SaveKey("Letters")).Split(',');
        if (letters.Length != cells.Count) return;

        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].SetLetter(letters[i]);
        }

        ShowWrongWords();

        // a saved puzzle that's already solved triggers the event again
        CheckIfComplete();
    }

    // each puzzle asset saves separately, so switching puzzles keeps the progress on each one
    string SaveKey(string name)
    {
        string puzzleName = puzzle != null ? puzzle.name : "Default";
        return "Crossword_" + puzzleName + "_" + name;
    }

    // right-click the script's header in the Inspector to run this, or use the button below it
    [ContextMenu("Clear Saved Letters")]
    public void ClearSavedLetters()
    {
        PlayerPrefs.DeleteKey(SaveKey("Grid"));
        PlayerPrefs.DeleteKey(SaveKey("Letters"));
        PlayerPrefs.DeleteKey(SaveKey("Revealed"));
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

    // ---------------- CLUES ----------------

    // read the clues, make the panels if needed, then show them
    // (they're made even when Show Clues is off, so it can be turned on in Play mode)
    void SetupClues()
    {
        ReadClues();

        if (acrossText == null)
        {
            acrossText = MakeCluePanel("Across Clues");
            acrossPanel = (RectTransform)acrossText.transform.parent;
        }
        if (downText == null)
        {
            downText = MakeCluePanel("Down Clues");
            downPanel = (RectTransform)downText.transform.parent;
        }
    }

    // split each "ANSWER: clue" line at its first colon
    void ReadClues()
    {
        clueByAnswer.Clear();
        foreach (string line in puzzle.clues.Split('\n'))
        {
            if (line.Trim() == "") continue;

            int colon = line.IndexOf(':');
            if (colon < 0)
            {
                Debug.LogWarning("Crossword: this clue needs \"ANSWER:\" at the start: " + line);
                continue;
            }
            string answer = line.Substring(0, colon).Trim().ToUpper();
            clueByAnswer[answer] = line.Substring(colon + 1).Trim();
        }

        // clues changed by SetClue() replace the ones in the asset
        foreach (KeyValuePair<string, string> changed in changedClues)
        {
            clueByAnswer[changed.Key] = changed.Value;
        }

        // point out typos, so a clue doesn't quietly go missing
        List<string> answers = new List<string>();
        foreach (List<Crossword_Cell> word in words)
        {
            answers.Add(GetAnswer(word));
        }
        if (!style.showClues) return;
        foreach (string answer in clueByAnswer.Keys)
        {
            if (!answers.Contains(answer)) Debug.LogWarning("Crossword: there's a clue for " + answer + ", but it isn't in the grid");
        }
        foreach (string answer in answers)
        {
            if (!clueByAnswer.ContainsKey(answer)) Debug.LogWarning("Crossword: " + answer + " doesn't have a clue");
        }
    }

    // make an empty panel with text in it. ApplyColors() styles it, and PlaceCluePanels() moves it
    TextMeshProUGUI MakeCluePanel(string panelName)
    {
        GameObject panel = new GameObject(panelName, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(transform.parent, false);
        panel.GetComponent<Image>().raycastTarget = false;

        // the text fills the panel. ApplyColors() sets the padding
        GameObject textObject = new GameObject("Text", typeof(RectTransform));
        textObject.transform.SetParent(panel.transform, false);
        RectTransform textRect = (RectTransform)textObject.transform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;

        // use the same font as the letters
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.font = cell.GetComponent<TMP_InputField>().textComponent.font;
        text.alignment = TextAlignmentOptions.TopLeft;
        // shrink the text when there are a lot of clues
        text.enableAutoSizing = true;
        text.raycastTarget = false;
        return text;
    }

    // put a panel on each side of the grid. This needs the grid's real size,
    // which the layout groups don't work out until the end of the frame, so it's called from LateUpdate()
    void PlaceCluePanels()
    {
        if (acrossPanel == null || downPanel == null) return;

        // update the layout now, so the grid can be measured
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);

        // find the grid's edges, measured in the same space as the panels
        RectTransform screen = (RectTransform)transform.parent;
        RectTransform gridRect = (RectTransform)row.transform.parent;
        RectTransform heightRect = style.matchGridHeight ? gridRect : (RectTransform)transform;
        Vector3[] gridCorners = new Vector3[4];
        Vector3[] heightCorners = new Vector3[4];
        gridRect.GetWorldCorners(gridCorners);
        heightRect.GetWorldCorners(heightCorners);
        // corner 0 is the bottom left, corner 2 is the top right
        float left = screen.InverseTransformPoint(gridCorners[0]).x;
        float right = screen.InverseTransformPoint(gridCorners[2]).x;
        float bottom = screen.InverseTransformPoint(heightCorners[0]).y;
        float top = screen.InverseTransformPoint(heightCorners[2]).y;

        float width = style.clueWidth * screen.rect.width;
        PlacePanel(acrossPanel, left - style.clueGap - width, left - style.clueGap, bottom, top);
        PlacePanel(downPanel, right + style.clueGap, right + style.clueGap + width, bottom, top);
    }

    // set a panel's edges, measured from the center of the screen
    void PlacePanel(RectTransform panel, float left, float right, float bottom, float top)
    {
        RectTransform screen = (RectTransform)transform.parent;
        // anchor the panel to the screen's pivot, so the numbers above mean the same thing for it
        panel.anchorMin = screen.pivot;
        panel.anchorMax = screen.pivot;
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = new Vector2(right - left, top - bottom);
        panel.anchoredPosition = new Vector2((left + right) / 2, (bottom + top) / 2);
    }

    // color everything, and size the clue text. Called at the start, and when the Inspector changes
    void ApplyColors()
    {
        Image background = GetComponent<Image>();
        if (background != null) background.color = style.backgroundColor;

        Transform gridTransform = row.transform.parent;
        Image gridImage = gridTransform.GetComponent<Image>();
        if (gridImage != null) gridImage.color = style.gridLineColor;

        // every row, including the hidden one that gets copied
        foreach (Transform child in gridTransform)
        {
            Image rowImage = child.GetComponent<Image>();
            if (rowImage != null && child != cell.transform) rowImage.color = style.rowColor;
        }

        foreach (Crossword_Cell crosswordCell in cells)
        {
            crosswordCell.SetColors(style.cellColor, style.blackSquareColor, style.letterColor, style.numberColor);
        }

        if (acrossText != null) acrossText.gameObject.SetActive(style.showClues);
        if (downText != null) downText.gameObject.SetActive(style.showClues);
        StylePanel(acrossPanel);
        StylePanel(downPanel);
    }

    // only the panels this script made are styled
    void StylePanel(RectTransform panel)
    {
        if (panel == null) return;

        panel.gameObject.SetActive(style.showClues);
        panel.GetComponent<Image>().color = style.clueBackgroundColor;

        TextMeshProUGUI text = panel.GetComponentInChildren<TextMeshProUGUI>(true);
        text.color = style.clueTextColor;
        text.fontSizeMin = style.clueFontSizeMin;
        text.fontSizeMax = Mathf.Max(style.clueFontSizeMin, style.clueFontSizeMax);
        text.paragraphSpacing = style.clueSpacing;

        RectTransform textRect = (RectTransform)text.transform;
        textRect.offsetMin = new Vector2(style.cluePadding, style.cluePadding);
        textRect.offsetMax = new Vector2(-style.cluePadding, -style.cluePadding);
    }

    // rebuild both clue lists from the clues, the letters typed so far, and the words revealed.
    // the grid never reads this text, so it's safe to call any time, as often as you like
    public void RefreshClues()
    {
        if (!style.showClues || acrossText == null || downText == null) return;

        HashSet<string> solved = new HashSet<string>();
        foreach (List<Crossword_Cell> word in words)
        {
            if (IsCorrect(word)) solved.Add(GetAnswer(word));
        }

        acrossText.text = BuildClueList("ACROSS", acrossWords, solved);
        downText.text = BuildClueList("DOWN", downWords, solved);
    }

    // a heading, then one line per word, like "2  Hides part of an image"
    string BuildClueList(string heading, List<List<Crossword_Cell>> list, HashSet<string> solved)
    {
        StringBuilder text = new StringBuilder();
        text.Append("<color=#" + Hex(style.clueHeadingColor, false) + "><b>" + heading + "</b></color>\n");

        foreach (List<Crossword_Cell> word in list)
        {
            string clue;
            if (!clueByAnswer.TryGetValue(GetAnswer(word), out clue)) clue = "";

            // a solved clue fades, and the clue being typed in uses the highlight color
            bool faded = IsCorrect(word);
            bool highlighted = word == highlightedWord;
            string numberHex = Hex(highlighted ? style.highlightColor : style.clueNumberColor, faded);
            string textHex = Hex(highlighted ? style.highlightColor : style.clueTextColor, faded);

            // the indent lines up a long clue's second line with its first
            string line =
                "<color=#" + numberHex + "><b>" + word[0].number + "</b></color>" +
                "<color=#" + textHex + "><indent=1.8em>" + HideWords(clue, solved, faded) + "</indent></color>";

            // cross out a solved clue
            if (faded) line = "<s>" + line + "</s>";

            // put a box behind the clue being typed in
            if (highlighted && style.highlightBackgroundColor.a > 0)
            {
                line = "<mark=#" + Hex(style.highlightBackgroundColor, false) + ">" + line + "</mark>";
            }

            text.Append(line + "\n");
        }
        return text.ToString();
    }

    // a color as text for a rich text tag, like "FF0000FF" for red. Faded colors use solvedAlpha
    string Hex(Color color, bool faded)
    {
        if (faded) color.a *= style.solvedAlpha;
        return ColorUtility.ToHtmlStringRGBA(color);
    }

    // cover each {key:words} with a bar, or show just the words once the key is revealed
    string HideWords(string clue, HashSet<string> solved, bool faded)
    {
        string barHex = Hex(style.hiddenColor, faded);

        return hiddenPattern.Replace(clue, match =>
        {
            string key = match.Groups[1].Value.Trim().ToUpper();
            string hiddenWords = match.Groups[2].Value;
            if (revealedKeys.Contains(key) || solved.Contains(key)) return hiddenWords;

            // swap the letters for x's (so they can't be read) the same color as the bar on top
            string covered = Regex.Replace(hiddenWords, @"\S", "x");
            return "<mark=#" + barHex + "><color=#" + barHex + ">" + covered + "</color></mark>";
        });
    }

    // the letters of a word, like "MASK"
    string GetAnswer(List<Crossword_Cell> word)
    {
        StringBuilder answer = new StringBuilder();
        foreach (Crossword_Cell crosswordCell in word)
        {
            answer.Append(crosswordCell.answer);
        }
        return answer.ToString();
    }

    // show the hidden clue words that use this key, like {key:words}. It's saved with the letters
    public void RevealClueWords(string key)
    {
        if (revealedKeys.Add(key.Trim().ToUpper()))
        {
            // the cells aren't made until Start, and saving before then would erase the saved letters
            if (cells.Count > 0) SaveLetters();
            RefreshClues();
        }
    }

    // cover the clue words that use this key again
    public void HideClueWords(string key)
    {
        if (revealedKeys.Remove(key.Trim().ToUpper()))
        {
            if (cells.Count > 0) SaveLetters();
            RefreshClues();
        }
    }

    // change a word's clue while the game is running. It can hide words with {key:words} too
    // (this doesn't change the puzzle asset, so the change only lasts until Play mode stops)
    public void SetClue(string answer, string clue)
    {
        answer = answer.Trim().ToUpper();
        changedClues[answer] = clue;
        clueByAnswer[answer] = clue;
        RefreshClues();
    }

    // every key used to hide clue words, like "PIXEL" in {PIXEL:GPU}
    public List<string> GetClueKeys()
    {
        List<string> keys = new List<string>();
        foreach (string clue in clueByAnswer.Values)
        {
            foreach (Match match in hiddenPattern.Matches(clue))
            {
                string key = match.Groups[1].Value.Trim().ToUpper();
                if (!keys.Contains(key)) keys.Add(key);
            }
        }
        return keys;
    }

    // a clue's name, like "3 Down", matching the number shown in the grid
    string ClueName(List<Crossword_Cell> word)
    {
        return word[0].number + (acrossWords.Contains(word) ? " Across" : " Down");
    }

    // find a word by its clue's name, like "3 Down" (capitals don't matter)
    List<Crossword_Cell> FindClue(string clueName)
    {
        List<Crossword_Cell> found = words.Find(word =>
            string.Equals(ClueName(word), clueName.Trim(), System.StringComparison.OrdinalIgnoreCase));
        if (found == null) Debug.LogWarning("Crossword: there's no clue called \"" + clueName + "\". Use a name like \"3 Down\"");
        return found;
    }

    // the names of the clues that hide words, like "3 Down", Across first and then Down
    public List<string> GetHiddenClueNames()
    {
        List<string> names = new List<string>();
        foreach (List<Crossword_Cell> word in words)
        {
            if (KeysInClue(word).Count > 0) names.Add(ClueName(word));
        }
        return names;
    }

    // the keys used in one clue, like "PIXEL" in 3 Down's {PIXEL:GPU}
    public List<string> GetClueKeys(string clueName)
    {
        List<Crossword_Cell> word = FindClue(clueName);
        return word != null ? KeysInClue(word) : new List<string>();
    }

    List<string> KeysInClue(List<Crossword_Cell> word)
    {
        List<string> keys = new List<string>();
        string clue;
        if (!clueByAnswer.TryGetValue(GetAnswer(word), out clue)) return keys;

        foreach (Match match in hiddenPattern.Matches(clue))
        {
            string key = match.Groups[1].Value.Trim().ToUpper();
            if (!keys.Contains(key)) keys.Add(key);
        }
        return keys;
    }

    // are all of this clue's hidden words showing?
    public bool IsClueRevealed(string clueName)
    {
        List<string> keys = GetClueKeys(clueName);
        return keys.Count > 0 && keys.TrueForAll(IsClueWordRevealed);
    }

    // show every hidden word in one clue, like "3 Down". (If another clue uses the same key, it shows there too)
    // a UI Button's On Click can call this, with the clue name typed in its box
    public void RevealClue(string clueName)
    {
        foreach (string key in GetClueKeys(clueName))
        {
            RevealClueWords(key);
        }
    }

    // cover one clue's hidden words again. A key named after an answer stays shown while that word is solved
    public void HideClue(string clueName)
    {
        foreach (string key in GetClueKeys(clueName))
        {
            HideClueWords(key);
        }
    }

    // are the clue words that use this key showing? (revealed, or the answer with that name is solved)
    public bool IsClueWordRevealed(string key)
    {
        key = key.Trim().ToUpper();
        if (revealedKeys.Contains(key)) return true;
        return words.Exists(word => GetAnswer(word) == key && IsCorrect(word));
    }

    // right-click the script's header in the Inspector (in Play mode) to run this, or use the button
    [ContextMenu("Reveal All Clue Words")]
    public void RevealAllClueWords()
    {
        foreach (string key in GetClueKeys())
        {
            RevealClueWords(key);
        }
    }

    // cover every hidden clue word again (except ones whose answer is solved)
    [ContextMenu("Hide All Clue Words")]
    public void HideAllClueWords()
    {
        revealedKeys.Clear();
        if (cells.Count > 0) SaveLetters();
        RefreshClues();
    }
}
