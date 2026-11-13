using UnityEngine;

/// <summary>
/// One crossword's answers and clues, saved as an asset so puzzles can be swapped on the Crossword component.
/// Make a new one with Assets > Create > Crossword > Puzzle
/// </summary>
[CreateAssetMenu(fileName = "Crossword_Puzzle", menuName = "Crossword/Puzzle")]
public class Crossword_Puzzle : ScriptableObject
{
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

    [Tooltip("One clue per line, written as ANSWER: clue. Across or Down comes from the grid.\n" +
        "Hide words with {key:hidden words}. They show when the word named by the key is solved, " +
        "when RevealClueWords(\"key\") is called, or when the \"crosswordReveal\" event is triggered with the key")]
    [MonospaceTextArea(12)]
    public string clues =
        "MASK: Hides part of an image\n" +
        "SPRITE: A 2D image used in a game\n" +
        "HUE: Red, green or blue, for example\n" +
        "RENDER: Draw the final image of a scene\n" +
        "FONT: A typeface for UI text\n" +
        "PIXEL: The smallest dot on a screen\n" +
        "MESH: The vertices, edges and faces of a 3D model\n" +
        "SHADER: Code that tells the {PIXEL:GPU} how to color a surface\n" +
        "EFFECT: A particle {SPRITE:explosion} is a visual ___\n" +
        "ART: Games are full of it\n" +
        "DOT: A tiny round mark";

    // hide words until something else in the game reveals them, using any key that isn't an answer:
    // "ART: Games are full of it, {portal:and so is this room}"
    // then from any script: EventManager.TriggerEvent("crosswordReveal", "portal");

    // the Crossword listens to this, so clue changes show up during Play mode
    public event System.Action changed;

    // Unity calls this when a value is changed in the Inspector
    void OnValidate()
    {
        changed?.Invoke();
    }
}
