using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Like [TextArea], but every character is the same width so text art lines up in the Inspector
/// </summary>
public class MonospaceTextAreaAttribute : PropertyAttribute
{
    public int minLines;

    public MonospaceTextAreaAttribute(int minLines = 3)
    {
        this.minLines = minLines;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(MonospaceTextAreaAttribute))]
public class MonospaceTextAreaDrawer : PropertyDrawer
{
    int LineCount(SerializedProperty property)
    {
        int minLines = ((MonospaceTextAreaAttribute)attribute).minLines;
        return Mathf.Max(minLines, property.stringValue.Split('\n').Length);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // one line for the label, then the text
        return EditorGUIUtility.singleLineHeight * (LineCount(property) + 1) + 4;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUIStyle style = new GUIStyle(EditorStyles.textArea);
        // the monospace font that comes with the Unity Editor
        style.font = EditorGUIUtility.Load("Fonts/RobotoMono/RobotoMono-Regular.ttf") as Font;

        Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect textRect = new Rect(position.x, labelRect.yMax, position.width, position.height - labelRect.height);

        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.LabelField(labelRect, label);
        EditorGUI.BeginChangeCheck();
        string text = EditorGUI.TextArea(textRect, property.stringValue, style);
        if (EditorGUI.EndChangeCheck())
        {
            property.stringValue = text;
        }
        EditorGUI.EndProperty();
    }
}
#endif
