# Crossword

A crossword built from text. Type in the squares, press **Tab** to move to the next letter, and read the clues beside the grid: **Across** on the left, **Down** on the right. When every letter is right, the `crosswordComplete` event is triggered.

Open `Crossword.unity`, or load it as an additive scene from `Trigger_Portals`.

**Contents**

1. [Quick start](#quick-start)
2. [Making a puzzle](#making-a-puzzle)
3. [Hidden clue words](#hidden-clue-words)
4. [Changing the look](#changing-the-look)
5. [Testing](#testing)
6. [Using it from other scripts](#using-it-from-other-scripts)
7. [Saving](#saving)
8. [Things to know](#things-to-know)
9. [Files](#files)

---

## Quick start

1. Open `Crossword.unity` and select the **Crossword** object.
2. Its **Puzzle** slot holds the answers and clues, and its **Style** slot holds the colors and layout. Both assets' settings show as fold-outs at the bottom of the Crossword's Inspector, so you can edit them there.
3. Press Play.

The grid and the clue panels are made by code when the game starts. **Don't edit the rows, cells or clue panels in the scene.** Change the Puzzle and Style assets instead.

---

## Making a puzzle

Make a new puzzle with **Assets > Create > Crossword > Puzzle**, then drag it into the Crossword's **Puzzle** slot. `Crossword_Puzzle_Small` is a 3-word example to copy from.

### Grid

Write one line per row of the crossword:

- Put a **space** between squares.
- Use **`_`** for a black square.
- Make every line the **same length**.

```
_ _ _ H O T _
_ _ _ A _ _ _
_ _ I T _ _ _
```

Any run of two or more letters, across or down, is a word. The numbers in the squares are worked out for you, going left to right, then top to bottom.

### Clues

Write one clue per line, starting with the answer and a colon:

```
HOT: Not cold
HAT: Worn on the head
IT: Tag, you're ___
```

- The order doesn't matter. Each clue goes under Across or Down, with the right number, based on where its answer is in the grid.
- Capitals in the answer don't matter.
- If a word has no clue, or a clue's answer isn't in the grid, a warning in the Console tells you which one. That's usually a typo.

---

## Hidden clue words

You can cover part of a clue with a bar until the player has done something. Wrap the words in braces, with a **key** before a colon:

```
HAT: Worn on the {bonus:head}
```

This shows as "Worn on the ████" until the key `bonus` is revealed. There are two kinds of key:

| Key | When the words show | Example |
|---|---|---|
| An **answer** in the grid | When that word is solved, so solving one word helps with another | `SHADER: Code that tells the {PIXEL:GPU} how to color a surface` |
| **Anything else** | When something reveals it: another script, an event or a button | `HAT: Worn on the {bonus:head}` |

- One clue can have more than one hidden part, and they can use different keys.
- Several clues can share a key. Revealing that key shows all of them.
- Revealed keys are saved with the letters.
- A key named after an answer can't be hidden again while that word is solved. Use a key that isn't an answer when you want full control.

### Revealing words

| Do this | From |
|---|---|
| `EventManager.TriggerEvent("crosswordReveal", "bonus");` | Any script. Best for game requirements, because the script doesn't need to find the Crossword. |
| `crossword.RevealClueWords("bonus");` | A script with a reference to the Crossword. Reveals every clue that uses the key. |
| `crossword.RevealClue("2 Across");` | A script, or a UI Button. Reveals every hidden part of one clue. |
| `crossword.HideClueWords("bonus");` / `crossword.HideClue("2 Across");` | Covers them again. |
| `crossword.RevealAllClueWords();` / `crossword.HideAllClueWords();` | Everything at once. |

A clue's name is its number and direction, as shown beside the grid, like `3 Down` or `2 Across`. Capitals don't matter.

**Example:** reveal a clue word when the player walks into a trigger:

```csharp
using UnityEngine;

public class RevealOnEnter : MonoBehaviour
{
    public string key = "bonus";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.TriggerEvent("crosswordReveal", key);
        }
    }
}
```

---

## Changing the look

Make a new style with **Assets > Create > Crossword > Style**, or edit `Crossword_Style_Default`. A style can be shared by many puzzles.

**Tip:** change the style during Play mode and the crossword updates right away. Changes to an asset are kept when Play mode stops. That's different from normal component values, which go back when you stop.

### Options

| Setting | What it does |
|---|---|
| Cell Spacing | Gap between squares and around the edge. Use a whole number so the gaps match. |
| Show Numbers | The small numbers in the squares. They're drawn anyway when Show Clues is on, because the clues need them. |
| Show Clues | The clue panels. Turn it off for a crossword without clues. You can toggle it in Play mode. |

### Grid colors

Background, Grid Line, Row, Cell, Black Square, Letter, Number, and Wrong (the letters of a finished word that's wrong).

The **Row** color is drawn over the grid lines behind each row. Set it to clear if you want every line to match.

These colors replace the ones on the scene objects, so change them here, not on the Images in the scene.

### Clue layout

| Setting | What it does |
|---|---|
| Clue Width | Width of each clue panel, as a fraction of the screen width. A wider panel makes the grid smaller. |
| Clue Gap | Pixels between the grid and each panel. |
| Match Grid Height | On: panels are as tall as the grid. Off: they're as tall as the whole crossword area, which helps with short, wide grids. |
| Clue Padding | Space between a panel's edge and its text. |
| Clue Font Size Min / Max | The text shrinks to fit, between these sizes. If clues are crowded, lower Min or widen the panels. |
| Clue Spacing | Extra space after each clue. |

### Clue colors

| Setting | What it does |
|---|---|
| Clue Background, Heading, Number, Text | The panel and its text. |
| Highlight | Text color of the clue for the word being typed in. |
| Highlight Background | A box behind that clue. Leave its alpha at 0 for no box. |
| Solved Alpha | How faded a solved clue is. Solved clues are also crossed out. |
| Hidden | The bar covering hidden words. |

### Using your own clue text

The Crossword's **Across Text** and **Down Text** slots are optional. Leave them empty and panels are made for you. Assign your own TextMeshPro objects and the clues are written into them, but they aren't moved, resized or recolored, and the clue layout settings don't apply.

---

## Testing

### Inspector buttons

Select the Crossword object during Play mode. Under **Test Hidden Clue Words** you'll find:

- One row per clue that hides words, like `3 Down (PIXEL)   shown`, with **Reveal** and **Hide** buttons.
- **Reveal All** and **Hide All**.
- **Event Key** and **Send**, which trigger the `crosswordReveal` event the same way another script would.
- **Clear Saved Letters**, which starts the puzzle fresh. It also works outside Play mode.

### Test buttons in the game

The **Test Buttons** object (under the Canvas) adds buttons under the grid when the game starts:

- One per clue that hides words, like **Reveal 3 Down**. The label changes to **Hide 3 Down** while those words are showing.
- **Reveal All** and **Hide All**. Turn these off with **All Buttons**.
- To choose which clues get buttons, type their names in the **Clues** list, like `3 Down`. Leave the list empty for all of them.

When you're done testing, **turn off or delete the Test Buttons object**. Nothing else needs it.

### Making your own button

1. Right-click the **Canvas** and choose **UI > Button - TextMeshPro**. Position it and change its label.
2. In the Button's **On Click ()** list, click **+**.
3. Drag the **Crossword** object into the empty slot.
4. From the function menu, choose **Crossword > RevealClue (string)** and type a clue name, like `3 Down`.

You can also choose `HideClue`, `RevealClueWords` (type a key, like `bonus`), `RevealAllClueWords` or `HideAllClueWords`. Any `public` method with no parameter, or one string, int, float or bool parameter, can be picked here.

---

## Using it from other scripts

### Events

| Event | Direction | Data |
|---|---|---|
| `crosswordComplete` | Sent by the crossword when every letter is right | none |
| `crosswordReveal` | Received by the crossword | the key to reveal, as a string |

Listen for the puzzle being finished:

```csharp
void OnEnable()  { EventManager.StartListening("crosswordComplete", OnSolved); }
void OnDisable() { EventManager.StopListening("crosswordComplete", OnSolved); }

void OnSolved()
{
    Debug.Log("The crossword is solved!");
}
```

A saved puzzle that's already solved sends `crosswordComplete` again when the scene loads.

### Crossword methods

| Method | What it does |
|---|---|
| `RevealClueWords(key)` / `HideClueWords(key)` | Show or cover every clue word using this key |
| `RevealClue(name)` / `HideClue(name)` | Show or cover one clue's hidden words, like `"3 Down"` |
| `RevealAllClueWords()` / `HideAllClueWords()` | Every key at once |
| `IsClueWordRevealed(key)` / `IsClueRevealed(name)` | Is it showing? |
| `GetClueKeys()` | Every key used in the clues |
| `GetHiddenClueNames()` | The names of the clues that hide words |
| `GetClueKeys(name)` | The keys used in one clue |
| `SetClue(answer, clue)` | Replace a clue while the game runs, like `SetClue("HAT", "A {bonus:fancy} cap")`. This doesn't change the asset, so it lasts until Play mode stops. |
| `RefreshClues()` | Redraw the clue lists. This happens on its own, but it's safe to call any time. |
| `ClearSavedLetters()` | Forget the saved letters and revealed keys for this puzzle |

---

## Saving

Letters and revealed keys are saved on this computer with `PlayerPrefs`, separately for each puzzle asset, so switching puzzles keeps the progress on each one.

- A finished word that's wrong isn't saved, unless its letters are also part of a correct crossing word.
- If the grid is changed, the old saved letters are ignored.
- To start over, use **Clear Saved Letters** in the Inspector, or right-click the Crossword component's header.

---

## Things to know

- **Grid changes need a restart.** The squares are made once, when the game starts. Edit the grid during Play mode and a warning reminds you to stop and start again. Clue and style changes show right away.
- **Missing assets:** if the Puzzle or Style slot is empty, the crossword uses the defaults written in `Crossword_Puzzle.cs` and `Crossword_Style.cs` and logs a warning. Selecting the Crossword object in the Editor fills empty slots with the `_Default` assets.
- **Additive scene:** when this scene is loaded on top of another one, its camera turns itself off so the other scene's camera is used.
- **The Tab key** moves through the current word, then on to the next word. Clicking a square that's in both an Across and a Down word starts with the Across word.

---

## Files

| File | What it is |
|---|---|
| `Crossword.cs` | Builds the grid and clues, checks answers, saves, and handles hidden clue words |
| `Crossword_Cell.cs` | One square: its answer, letter, number and colors |
| `Crossword_Puzzle.cs` | The Puzzle asset type (grid and clues) |
| `Crossword_Style.cs` | The Style asset type (colors and layout) |
| `Crossword_TestButtons.cs` | The in-game test buttons |
| `Editor/Crossword_Editor.cs` | Adds the asset fold-outs and test buttons to the Crossword's Inspector. Editor-only, so it isn't included in builds. |
| `MonospaceTextAreaAttribute.cs` | Shows the grid and clues in a fixed-width font in the Inspector, so the columns line up |
| `Crossword_Puzzle_Default.asset`, `Crossword_Puzzle_Small.asset` | Example puzzles |
| `Crossword_Style_Default.asset` | The default look |
