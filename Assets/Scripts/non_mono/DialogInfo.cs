using System;
using System.Collections.Generic;
using UnityEditor.UI;

[Serializable]
public class DialogInfo {

    //enums-------------------------------------------------------
    //statics-----------------------------------------------------


    //fields------------------------------------------------------
    private readonly List<DialogParticipant> _participants;

    //properties--------------------------------------------------
    public List<DialogParticipant> Participants
    {
        get { return _participants; }
    }
    public List<String> DialogEntries { get; private set; }


    //ctors-------------------------------------------------------
    public DialogInfo()
    {
        _participants = new List<DialogParticipant>();
    }

    //methods-----------------------------------------------------





}