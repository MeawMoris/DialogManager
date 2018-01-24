using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DialogSettings))]
public class DialogSettingsDrawer : PropertyDrawer
{
    private bool foldoutValue;
    private float height= EditorGUIUtility.singleLineHeight;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //--------------------------------------------------------------------------------------------------------
        var _multipleSimultaneousDialogs = property.FindPropertyRelative("_multipleSimultaneousDialogs");
        var _showParticimantNames = property.FindPropertyRelative("_showParticimantNames");
        var _showParticipantImages = property.FindPropertyRelative("_showParticipantImages");
        var _autoPlay = property.FindPropertyRelative("_autoPlay");
        //--------------------------------------------------------------------------------------------------------
        var rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;
        height = EditorGUIUtility.singleLineHeight;
        //--------------------------------------------------------------------------------------------------------

        foldoutValue = EditorGUI.Foldout(rect,foldoutValue, "Dialog Settings");

        if (foldoutValue)
        {
            rect.height = EditorGUIUtility.singleLineHeight * 5;
            height = rect.height;
        }
        else        
            rect.height = EditorGUIUtility.singleLineHeight;

        //--------------------------------------------------------------------------------------------------------


        EditorGUI.BeginProperty(rect, GUIContent.none, property);
        var oldColor = GUI.backgroundColor;
        GUI.backgroundColor= Color.red;
        GUI.Box(rect, GUIContent.none);
        GUI.backgroundColor = oldColor;

        if (foldoutValue)
        {
            rect.height = EditorGUIUtility.singleLineHeight;

            rect.y += rect.height;
            _multipleSimultaneousDialogs.boolValue =
                EditorGUI.ToggleLeft(rect, _multipleSimultaneousDialogs.displayName,
                    _multipleSimultaneousDialogs.boolValue);

            rect.y += rect.height;
            _showParticimantNames.boolValue =
                EditorGUI.ToggleLeft(rect, _showParticimantNames.displayName, _showParticimantNames.boolValue);

            rect.y += rect.height;
            _showParticipantImages.boolValue =
                EditorGUI.ToggleLeft(rect, _showParticipantImages.displayName, _showParticipantImages.boolValue);

            rect.y += rect.height;
            _autoPlay.boolValue =
                EditorGUI.ToggleLeft(rect, _autoPlay.displayName, _autoPlay.boolValue);
            
        }

        EditorGUI.EndProperty();
        //--------------------------------------------------------------------------------------------------------

    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return  height;
    }
}