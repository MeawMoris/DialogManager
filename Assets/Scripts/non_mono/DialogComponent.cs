using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class DialogComponent:MonoBehaviour
{
    [SerializeField]
    private TextAsset _xmlDialogFile;

    [SerializeField]
    private DialogEntry _info;

  //  public image
    void Update()
    {
        
    }





}

public class DialogEntry
{
    [SerializeField][ParticipantSelector] private IList<DialogParticipant> _participants;

}

public class ParticipantSelector : PropertyAttribute
{
    public ParticipantSelector()
    {
        
    }
}





public class DialogManagerWindow : EditorWindow
{

    [MenuItem("Tools/Clear PlayerPrefs")]
    static void ViewWindow()
    {
        GetWindow<DialogManagerWindow>().Show();
    }


    private bool foldout_participants;

    private void OnEnable()
    {

    }

    private void OnGUI()
    {
        var rect =EditorGUIUtility.ScreenToGUIRect( this.position);
        Debug.Log(rect);
        var rect2 = rect;
        rect2.height *= .5f;
        rect2.x = rect2.y = 0;

        EditorGUILayout.("participants", foldout_participants);

        EditorGUILayout.TextField("test", "");
        EditorGUILayout.TextField("test", "");
        EditorGUILayout.TextField("test", "");
        EditorGUILayout.TextField("test", "");

        EditorGUILayout.EndToggleGroup();


        foldout_participants = EditorGUILayout.Foldout(foldout_participants, new GUIContent("participants"));

    }


}