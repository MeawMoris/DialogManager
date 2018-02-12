using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(ParticipantSelectorList))]
public class ParticipantSelectorListDrawer : PropertyDrawer
{

    private ReorderableList _reorderableList;


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        var ParticipantSelectorInfoList = property.FindPropertyRelative("_participantNames");

        var allParticipantNames =
            ParticipantsUtility.GetParticipantNames(DialogManager.GetInstance().GetAllParticipants());




        if (_reorderableList == null)
        {
            _reorderableList = new ReorderableList(property.serializedObject, ParticipantSelectorInfoList);
            _reorderableList.drawElementCallback += (rect, index2, active, focused) =>
            {
                var item = ParticipantSelectorInfoList.GetArrayElementAtIndex(index2);
                var itemParticipantName = item.FindPropertyRelative("participantName");
                var popupIndex = item.FindPropertyRelative("participantPopupIndex").intValue;
                int index = allParticipantNames.IndexOf(itemParticipantName.stringValue);

                if (!allParticipantNames.Contains(itemParticipantName.stringValue))
                {
                    itemParticipantName.stringValue = DialogManager.GetInstance().AnonymousParticipant.Name;
                    popupIndex = index = allParticipantNames.IndexOf(itemParticipantName.stringValue);

                }
                else if (popupIndex == -1)
                {
                    popupIndex = index = allParticipantNames.IndexOf(itemParticipantName.stringValue);
                }


                rect.height = EditorGUIUtility.singleLineHeight;
                popupIndex = EditorGUI.Popup(rect, "Index "+ index2, index, allParticipantNames.ToArray());
                if (popupIndex != index)
                {
                    index = popupIndex;
                    itemParticipantName.stringValue = allParticipantNames[index];
                }

                item.FindPropertyRelative("participantPopupIndex").intValue = popupIndex;


            };
            _reorderableList.onAddCallback += listt =>
            {
                ParticipantSelectorInfoList.InsertArrayElementAtIndex(ParticipantSelectorInfoList.arraySize);
                var item = ParticipantSelectorInfoList.GetArrayElementAtIndex(ParticipantSelectorInfoList.arraySize-1);
                item.FindPropertyRelative("participantName").stringValue="";
                item.FindPropertyRelative("participantPopupIndex").intValue = -1;
            };
            _reorderableList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, label);
            };
        }

        _reorderableList.DoList(position);

        RemoveDuplicates(property);


    }

    private void RemoveDuplicates(SerializedProperty property)
    {
        var listt = property.FindPropertyRelative("_participantNames");
        var count = listt.arraySize;

        List<string> filteredRows = new List<string>();
        for (int i = 0; i < count; i++)
        {
            var item = listt.GetArrayElementAtIndex(i);
            var itemParticipantName = item.FindPropertyRelative("participantName");
            filteredRows.Add(itemParticipantName.stringValue);
        }


        for (var i = 0; i < count; i++)
        {
            filteredRows.RemoveAt(0);
            var item = listt.GetArrayElementAtIndex(i);
            var itemParticipantName = item.FindPropertyRelative("participantName");

            if (filteredRows.Contains(itemParticipantName.stringValue))
            {               
                listt.DeleteArrayElementAtIndex(i);
                i--;
                count--;
            }
        }

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (_reorderableList != null)
            return base.GetPropertyHeight(property, label) + _reorderableList.GetHeight();
        return base.GetPropertyHeight(property, label);
    }
}