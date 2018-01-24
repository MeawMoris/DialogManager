using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DialogEntry))]
public class DialogEntryDrawer : PropertyDrawer
{
    private List<string> participantPopupNames = new List<string>();
    private Vector2 textScrollerPos;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;

        var _selectedParticipantIndex = property.FindPropertyRelative("_selectedParticipantIndex");
        var _participants = property.FindPropertyRelative("_participants");
        var _text = property.FindPropertyRelative("_text");

        //GetParticipantNames
        participantPopupNames.Clear();
        for (int i = 0; i < _participants.arraySize; i++)
        {
            var objectReferenceValue =
                _participants.GetArrayElementAtIndex(i).objectReferenceValue as DialogParticipant;
            participantPopupNames.Add(objectReferenceValue.Name);
        }

        //show Participants popup
        _selectedParticipantIndex.intValue =
            EditorGUI.Popup(rect, "Participant", _selectedParticipantIndex.intValue, participantPopupNames.ToArray());

        //show dialog text
        rect.y += rect.height;
        rect.height = EditorGUIUtility.singleLineHeight * 5;
        _text.stringValue = EditorGUI.TextArea(rect, _text.stringValue);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight * 6;
    }
}