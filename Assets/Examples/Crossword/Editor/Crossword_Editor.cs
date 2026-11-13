using UnityEditor;
using UnityEngine;

/// <summary>
/// Adds to the Crossword's Inspector: the Puzzle and Style settings (so you don't have to click into
/// each asset), and buttons to test hiding and revealing clue words in Play mode.
/// It's in an Editor folder, so it isn't included when the game is built
/// </summary>
[CustomEditor(typeof(Crossword))]
public class Crossword_Editor : Editor
{
    // the Inspectors for the two assets, drawn inside this one
    Editor puzzleEditor;
    Editor styleEditor;

    // are the asset settings open? (static, so they stay open when you select something else)
    static bool showPuzzle = true;
    static bool showStyle = false;

    // the key typed in to test the "crosswordReveal" event
    string eventKey = "";

    // fill in empty asset slots when the Crossword is selected, so the scene works right away
    void OnEnable()
    {
        serializedObject.Update();
        bool filled = FillIfEmpty<Crossword_Puzzle>("puzzle");
        filled |= FillIfEmpty<Crossword_Style>("style");
        if (filled) serializedObject.ApplyModifiedProperties();
    }

    // if the slot is empty, use an asset of this type, choosing one whose name ends in "Default" first
    bool FillIfEmpty<T>(string propertyName) where T : ScriptableObject
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property.objectReferenceValue != null) return false;

        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
        if (guids.Length == 0) return false;

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        foreach (string guid in guids)
        {
            string otherPath = AssetDatabase.GUIDToAssetPath(guid);
            if (otherPath.EndsWith("Default.asset")) path = otherPath;
        }

        property.objectReferenceValue = AssetDatabase.LoadAssetAtPath<T>(path);
        return true;
    }

    // keep the "shown" and "hidden" labels up to date while the game runs
    public override bool RequiresConstantRepaint()
    {
        return Application.isPlaying;
    }

    void OnDisable()
    {
        DestroyImmediate(puzzleEditor);
        DestroyImmediate(styleEditor);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Crossword crossword = (Crossword)target;
        DrawAsset(crossword.puzzle, ref puzzleEditor, ref showPuzzle);
        DrawAsset(crossword.style, ref styleEditor, ref showStyle);

        DrawTestButtons(crossword);
    }

    // a fold-out header with the asset's own settings under it.
    // these change the asset itself, so they're kept after Play mode stops (and shared by everything using it)
    void DrawAsset(Object asset, ref Editor assetEditor, ref bool open)
    {
        if (asset == null) return;

        EditorGUILayout.Space();
        open = EditorGUILayout.InspectorTitlebar(open, asset);
        if (!open) return;

        CreateCachedEditor(asset, null, ref assetEditor);
        assetEditor.OnInspectorGUI();
    }

    void DrawTestButtons(Crossword crossword)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Test Hidden Clue Words", EditorStyles.boldLabel);

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter Play mode to test revealing and hiding clue words.", MessageType.Info);
            if (GUILayout.Button("Clear Saved Letters")) crossword.ClearSavedLetters();
            return;
        }

        // one row per clue that hides words, like "3 Down", with the keys it uses
        foreach (string clueName in crossword.GetHiddenClueNames())
        {
            string keys = string.Join(", ", crossword.GetClueKeys(clueName));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(clueName + " (" + keys + ")", "The key in {key:words}"),
                new GUIContent(crossword.IsClueRevealed(clueName) ? "shown" : "hidden"));
            if (GUILayout.Button("Reveal", GUILayout.Width(60))) crossword.RevealClue(clueName);
            if (GUILayout.Button(new GUIContent("Hide", "A key named after an answer stays shown while that word is solved"),
                GUILayout.Width(60))) crossword.HideClue(clueName);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reveal All")) crossword.RevealAllClueWords();
        if (GUILayout.Button("Hide All")) crossword.HideAllClueWords();
        EditorGUILayout.EndHorizontal();

        // test the event the way another script would send it
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        eventKey = EditorGUILayout.TextField("Event Key", eventKey);
        if (GUILayout.Button("Send", GUILayout.Width(60)))
        {
            EventManager.TriggerEvent("crosswordReveal", eventKey);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.HelpBox("Send does the same as EventManager.TriggerEvent(\"crosswordReveal\", \"" +
            eventKey + "\") in a script.", MessageType.None);

        EditorGUILayout.Space();
        if (GUILayout.Button("Clear Saved Letters")) crossword.ClearSavedLetters();
    }
}
