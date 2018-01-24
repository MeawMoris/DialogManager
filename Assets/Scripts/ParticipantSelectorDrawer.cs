using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ParticipantSelector))]
public class ParticipantSelectorDrawer : PropertyDrawer
{

    private int popupIndex = -1;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //int popupIndex = -1;
        var posValue = property.FindPropertyRelative("_selectedItemName");
        var allParticipantNames =
            ParticipantsUtility.GetParticipantNames(DialogManager.GetInstance().GetAllParticipants());
        int index = allParticipantNames.IndexOf(posValue.stringValue);


        //if the Participant was removed from the list
        if (!allParticipantNames.Contains(posValue.stringValue))
        {
            posValue.stringValue = DialogManager.GetInstance().AnonymousParticipant.Name;
            popupIndex = index = allParticipantNames.IndexOf(posValue.stringValue);

        }
        else if (popupIndex == -1)
        {
            popupIndex = index = allParticipantNames.IndexOf(posValue.stringValue);
        }



        position.height = EditorGUIUtility.singleLineHeight;
        popupIndex = EditorGUI.Popup(position, index, allParticipantNames.ToArray());
        if (popupIndex != index)
        {
            index= popupIndex;
            posValue.stringValue = allParticipantNames[index];
        }


    }


}