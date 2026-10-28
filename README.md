# Art for Games

Examples for DIG 255 Art for Games



## Students: copy an example into your project

Every example lives in its own folder in `Assets/Examples/`. The scripts and prefabs that several examples share live in `Assets/_Core/`. To use an example, you always export **two folders together**: the example's folder and `_Core`.



<details>
<summary>Instructions</summary>

### 1. Before you start

Your project needs:

- **Unity 6** (this project uses 6000.6), made from the **Universal 2D** template
- The **Input System** package (Window → Package Manager). Set Edit → Project Settings → Player → Active Input Handling to **Input System Package (New)** or **Both**
- **TextMesh Pro**, if the example has text. Unity asks to import "TMP Essentials" the first time you open a scene that uses it. Click Import.

### 2. Export from this project

1. In the **Project** window, click the example folder, for example `Assets/Examples/Trigger_Text`.
2. Hold **Cmd** (Mac) or **Ctrl** (Windows) and also click `Assets/_Core`.
3. Right-click → **Export Package…**
4. Leave **Include dependencies** checked. If the list shows anything that starts with `Packages/`, uncheck it. Your project already gets those from the Package Manager.
5. Click **Export…** and save the `.unitypackage` file.

> Why both folders? Exporting only a scene can leave out scripts that other scripts need, and your project won't compile. The `_Core` folder is small, and adding all of it avoids that problem.

### 3. Import into your project

1. Open your project, then go to **Assets → Import Package → Custom Package…** and choose the file.
2. Click **Import**.
3. Open the scene in `Assets/Examples/<Example>/` and press Play.

### 4. Add tags and layers

Tags, layers and sorting layers are **project settings**, so they don't travel inside a package. Add any your example needs in **Edit → Project Settings → Tags and Layers**. The names must match exactly.

| Example | Tags | Layers | Sorting Layers |
|---|---|---|---|
| Trigger_Text | *(Player is built in)* | Ground | Player, Objects |
| Trigger_Sound, Trigger_Animation, Trigger_Portals, WalkingFigure | *(Player is built in)* | Ground | Player |
| Puzzle_Jigsaw | | Interactable | |
| Repeating_Background | Ground, Obstacle | | |
| RhythmGame | NoteChecker | | Player |

After you add them, check that the objects in the scene still use them. For example, the floor should be on the **Ground** layer, and the player's Ground Layer field should be set to **Ground**.

**Trigger_Portals:** a portal loads the scene named in its **Go To Scene** field. Add that scene to **File → Build Profiles → Scene List**, or nothing happens.

### 5. Changing the shared code

If you import a second example later, its `_Core` folder **replaces** yours, and any changes you made to `_Core` scripts are lost. To change a `_Core` script, first copy it into your own folder and rename the class.



## How this project is organized

```
Assets/
  _Core/              shared, reusable: scripts, prefabs, and art used by more than one example
    Events/ Input/ Player/ Triggers/ Puzzles/ Time/ Art/
  Examples/
    <Example_Name>/   one folder per demo: its scene(s) and anything only it uses
```

- The example folder, its scene and its main prefab all share one name (`Trigger_Text/Trigger_Text.unity`).
- A script or asset moves into `_Core` as soon as a second example needs it. Until then it stays in its example's folder.
- Nothing in `_Core` may reference anything in `Examples/`, so "example folder + `_Core`" is always a complete package.
- Move and rename assets **inside Unity** (or move each `.meta` file along with its asset), so that references don't break.

</details>



## Triggers

Reusable scripts and prefabs with parameters set in inspector and colliders to trigger animations, sounds, text, and "Portals" to move between scenes

![scenes](_screenshots/scenes.png)




## Repeat Background

![repeat](_screenshots/repeat-background.png)



## UI Examples

Trigger text from colliders

![repeat](_screenshots/ui-text-trigger.png)
