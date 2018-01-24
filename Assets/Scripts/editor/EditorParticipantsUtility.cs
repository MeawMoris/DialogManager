using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorParticipantsUtility
{
    public static List<string> GetParticipantSpriteNames(SerializedProperty participant)
    {
        var listt = participant.FindPropertyRelative("_sprites");
        var count = listt.arraySize;
        List<string> names = new List<string>();

        for (int i = 0; i < count; i++)
        {
            var sprite = listt.GetArrayElementAtIndex(i).objectReferenceValue as Sprite;
            if (sprite != null)
                names.Add(sprite.name);
        }
        return names;
    }

    public static List<string> GetParticipantNames(SerializedProperty participantList)
    {
        var count = participantList.arraySize;
        List<string> names = new List<string>();

        for (int i = 0; i < count; i++)
        {
            var name = participantList.GetArrayElementAtIndex(i).FindPropertyRelative("_name").stringValue;
            names.Add(name);
        }
        return names;
    }
}