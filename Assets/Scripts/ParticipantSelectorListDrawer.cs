using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(ParticipantSelectorList))]
public class ParticipantSelectorListDrawer : PropertyDrawer
{

    private int popupIndex = -1;
    private ReorderableList _reorderableList;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        var list = property.FindPropertyRelative("_selectors");
        if (_reorderableList == null)
        {
            _reorderableList = new ReorderableList(property.serializedObject,list);
            _reorderableList.drawElementCallback += (rect, index, active, focused) =>
            {
                EditorGUI.PropertyField(rect, list.GetArrayElementAtIndex(index), new GUIContent(list.displayName));
            };
            _reorderableList.onAddCallback += listt =>
            {
                list.InsertArrayElementAtIndex(list.arraySize);
                list.GetArrayElementAtIndex(list.arraySize-1).FindPropertyRelative("_selectedItemName").stringValue="";
            };
            _reorderableList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, label);
            };
        }

        _reorderableList.DoList(position);

    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (_reorderableList != null)
            return base.GetPropertyHeight(property, label) + _reorderableList.GetHeight();
        return base.GetPropertyHeight(property, label);
    }
}