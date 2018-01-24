using System.Collections;
using System.Collections.Generic;
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

public class Dialog
{
    [SerializeField] protected DialogSettings settings;
    [SerializeField] [ParticipantSelector] protected List<DialogParticipant> _participants;
    [SerializeField] protected List<DialogEntry> _dialogEntries;


    //dialog view window:
    /*
     * --add participants
     * --participants image settings
     * --dialog entries 
     */
}


public class blabla
{
    public enum ParticipantPosition
    {
        Auto,
        Left,
        Right,
    }
    [SerializeField] private ParticipantPosition _participantImagesPos;
    [SerializeField] private int _participantSpriteIndex;


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