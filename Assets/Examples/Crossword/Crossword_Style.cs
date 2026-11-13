using UnityEngine;

/// <summary>
/// How a crossword looks: colors, spacing and clue layout, saved as an asset so it can be shared by puzzles.
/// Make a new one with Assets > Create > Crossword > Style
/// Changes show up right away in Play mode, and (unlike component values) they're kept when Play mode stops
/// </summary>
[CreateAssetMenu(fileName = "Crossword_Style", menuName = "Crossword/Style")]
public class Crossword_Style : ScriptableObject
{
    [Tooltip("The gap between cells and around the outer edge. Use a whole number so they match")]
    public float cellSpacing = 3;

    [Tooltip("Show a small number in the first cell of each word")]
    public bool showNumbers = true;

    [Header("Grid Colors")]

    [Tooltip("The background behind the whole crossword area")]
    public Color backgroundColor = new Color(1, 1, 1, 0.392f);

    [Tooltip("The lines between the cells (the grid shows through the gaps)")]
    public Color gridLineColor = new Color(0.5f, 0.5f, 0.5f);

    [Tooltip("Drawn behind each row, on top of the grid lines. Make it clear so every line matches")]
    public Color rowColor = new Color(1, 1, 1, 0.392f);

    [Tooltip("The squares you type in")]
    public Color cellColor = Color.white;

    [Tooltip("The squares you can't type in")]
    public Color blackSquareColor = Color.black;

    [Tooltip("The letters typed in the squares")]
    public Color letterColor = new Color(0.196f, 0.196f, 0.196f);

    [Tooltip("The small numbers in the squares")]
    public Color numberColor = new Color(0.196f, 0.196f, 0.196f);

    [Tooltip("The letter color in a finished word that's wrong")]
    public Color wrongColor = Color.red;

    [Header("Clues")]

    [Tooltip("Show the Across and Down clue lists beside the grid. Clues need the numbers, so they're drawn even if Show Numbers is off")]
    public bool showClues = true;

    [Header("Clue Layout (for the panels made by the Crossword script)")]

    [Tooltip("The width of each clue panel, as a fraction of the screen width. The grid shrinks to make room")]
    [Range(0.05f, 0.4f)]
    public float clueWidth = 0.2f;

    [Tooltip("The space between the grid and each clue panel, in pixels")]
    public float clueGap = 20;

    [Tooltip("On: the panels are as tall as the grid. Off: they're as tall as the whole crossword area")]
    public bool matchGridHeight = true;

    [Tooltip("The space between a panel's edge and its text, in pixels")]
    public float cluePadding = 12;

    [Tooltip("The text shrinks to fit the panel, between these two sizes")]
    public float clueFontSizeMin = 10;
    public float clueFontSizeMax = 24;

    [Tooltip("Extra space after each clue, in font units")]
    public float clueSpacing = 8;

    [Header("Clue Colors")]

    [Tooltip("The panel behind the clues")]
    public Color clueBackgroundColor = new Color(1, 1, 1, 0.392f);

    [Tooltip("The ACROSS and DOWN headings")]
    public Color clueHeadingColor = new Color(0.196f, 0.196f, 0.196f);

    [Tooltip("The number before each clue")]
    public Color clueNumberColor = new Color(0.196f, 0.196f, 0.196f);

    [Tooltip("The clue text")]
    public Color clueTextColor = new Color(0.196f, 0.196f, 0.196f);

    [Tooltip("The text color of the clue for the word being typed in")]
    public Color highlightColor = new Color(0.1f, 0.4f, 0.9f);

    [Tooltip("A box behind the clue for the word being typed in. Leave it clear (alpha 0) for no box")]
    public Color highlightBackgroundColor = new Color(1, 0.85f, 0.3f, 0);

    [Tooltip("How see-through a solved clue is (0 is invisible, 1 is solid)")]
    [Range(0, 1)]
    public float solvedAlpha = 0.35f;

    [Tooltip("The color of the bar covering hidden clue words")]
    public Color hiddenColor = new Color(0.196f, 0.196f, 0.196f);

    // the Crossword listens to this, so changes show up during Play mode
    public event System.Action changed;

    // Unity calls this when a value is changed in the Inspector
    void OnValidate()
    {
        changed?.Invoke();
    }
}
