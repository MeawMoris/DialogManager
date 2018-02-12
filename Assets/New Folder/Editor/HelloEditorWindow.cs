using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HelloEditorWindow : EditorWindow
{
    HelloDataForEditor data;

    [MenuItem("Window/Hello Window")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        HelloEditorWindow window = (HelloEditorWindow)EditorWindow.GetWindow(typeof(HelloEditorWindow));
        window.Show();
    }

    //void OnGUI()
    //{
    //    if (data == null)
    //    {
    //        data = new HelloDataForEditor();
    //    }

    //    if (GUILayout.Button("Add Hello"))
    //    {
    //        data.List.Add(new HelloWorldObject());
    //    }

    //    SerializedObject o = new SerializedObject(data);

    //    SerializedProperty single = o.FindProperty("NonListed");
    //    EditorGUILayout.PropertyField(single, true);

    //    SerializedProperty list = o.FindProperty("List");
    //    for (int i = 0; i < list.arraySize; i++)
    //    {
    //        EditorGUILayout.LabelField(list.GetArrayElementAtIndex(i).type);
    //        EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), true);
    //    }
    //}

    void OnGUI()
    {
        if (data == null)
        {
            data = ScriptableObject.CreateInstance<HelloDataForEditor>();
        }

        if (GUILayout.Button("Add Hello"))
        {
            data.List.Add(new HelloWorldObject());
        }

        var editor = Editor.CreateEditor(data);
        if (editor != null)
        {
            editor.OnInspectorGUI();
        }
    }
}