using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class DialogComponent : MonoBehaviour
{
    // [SerializeField]
    // private TextAsset _xmlDialogFile;

    // [SerializeField]
    //private Dialog _info;

    //  public image
    void Update()
    {
    }
}




[Serializable]
public class Dialog
{
    [SerializeField] DialogSettings _dialogSettings;
    [SerializeField]  ParticipantSelectorList _participantSelectors;
    [SerializeField]  List<DialogParticipantImageSettings> _participantsImageSettings = new List<DialogParticipantImageSettings>();
    [SerializeField]  List<DialogEntry> _dialogEntries;


    //dialog view window:
    /*
     * --add participants
     * --participants image settings
     * --dialog entries 
     */



/*    /// <summary>
    /// starts a dialog if none is stated.
    /// 
    /// returns false id another dialog is taking place and "multi-dialog" feature is disabled
    ///  or if dialogs are disabled.
    /// </summary>
    /// <returns>returns true if the dialog started, otherwise returns false</returns>
    public bool StartDialog()
    {
        //todo recieve dialogData
        //todo return false if a multi dialog is displaying
        //todo show multiple dialogs if the feature is enabled
        return false;
    }




    //todo implement events invokers 
    //todo invoke events at the right moment
    public event EventHandler OnDialogStart;
    public event EventHandler OnDialogEnd;
    public event EventHandler OnSlideChange;
    public bool IsDialogRunnig { get; private set; }*/



}

[CustomPropertyDrawer(typeof(Dialog))]
public class DialogDrawer : PropertyDrawer
{
    private float height = EditorGUIUtility.singleLineHeight;
    private List<DialogParticipant> _allParticipants;
    private bool participantSelectorsFoldout;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        _allParticipants = DialogManager.GetInstance().GetAllParticipants();
        //--------------------------------------------------------------------------------------------------------
        var _dialogSettings = property.FindPropertyRelative("_dialogSettings");
        var _participantSelectors = property.FindPropertyRelative("_participantSelectors");
        var _participantsImageSettings = property.FindPropertyRelative("_participantsImageSettings");
        var _dialogEntries = property.FindPropertyRelative("_dialogEntries");
        //----------------------------------------------------------------------------------------
        var rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;
        height = 0;
        //--------------------------------------------------------------------------------------------------------
        DrawDialogSettings(_dialogSettings,ref rect);
        //--------------------------------------------------------------------------------------------------------
        participantSelectorsFoldout = EditorGUI.Foldout(rect, participantSelectorsFoldout,"Select Participants In Conversation");

        rect.y += EditorGUIUtility.singleLineHeight;
        //height += EditorGUIUtility.singleLineHeight;
        if(participantSelectorsFoldout)
            DrawParticipantSelectors(_participantSelectors, ref rect);
        else height += EditorGUIUtility.singleLineHeight;
        //--------------------------------------------------------------------------------------------------------


    }

    private void DrawDialogSettings(SerializedProperty _dialogSettings, ref Rect rect)
    {

        // base.OnGUI(position, property, label);
        EditorGUI.PropertyField(rect, _dialogSettings, new GUIContent(_dialogSettings.displayName), true);
        var propertyHeight = EditorGUI.GetPropertyHeight(_dialogSettings, true);
        height += propertyHeight;
        rect.y += propertyHeight;
    }
    private void DrawParticipantSelectors(SerializedProperty _participantSelectors,ref Rect rect)
    {

        // base.OnGUI(position, property, label);
        EditorGUI.PropertyField(rect, _participantSelectors, new GUIContent(_participantSelectors.displayName), true);
        var propertyHeight = EditorGUI.GetPropertyHeight(_participantSelectors, true);
        height += propertyHeight;
        rect.y += propertyHeight;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return  height;
    }
    

}



[Serializable]
public class DialogParticipantImageSettings
{
    public enum ParticipantPosition
    {
        Auto,
        Left,
        Right,
    }


    [SerializeField] private string ParticipantName;
    [SerializeField] private ParticipantPosition _participantImagesPos = ParticipantPosition.Auto;
    [SerializeField] private int _participantSpriteIndex = 0;


    public ParticipantPosition ParicipantImagesPos
    {
        get { return _participantImagesPos; }
        set { _participantImagesPos = value; }
    }
    public int ParticipantSpriteIndex
    {
        get { return _participantSpriteIndex; }
        set { _participantSpriteIndex = value; }
    }

}


public class ParticipantSelectorAttribute : PropertyAttribute
{
//todo to implement class
    public ParticipantSelectorAttribute()
    {
    }
}