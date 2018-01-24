using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

[Serializable]
public class DialogEntry
{
    private static List<DialogParticipant> _defaultParticipantList;
    public static ReadOnlyCollection<DialogParticipant> GetDefaultParticipantList()
    {
        if (_defaultParticipantList == null)
        {
            _defaultParticipantList = new List<DialogParticipant>();
            //todo add default(anonymous) Participant
        }
        return _defaultParticipantList.AsReadOnly();
    }


    [SerializeField] private List<DialogParticipant> _participants;
    [SerializeField] private int _selectedParticipantIndex;
    [SerializeField] private int _participantImageIndex;
    [SerializeField] private string _text;
    //todo add Participant sprite field

    public DialogEntry(List<DialogParticipant> participants)
    {
        ParticipantsList = participants;
    }


    public string Text
    {
        get { return _text; }
    }

    public DialogParticipant SelectedParticipant
    {
        get
        {
            return ParticipantsList[_selectedParticipantIndex];
        }
    }
    public List<DialogParticipant> ParticipantsList
    {
        get { return _participants; }
        set
        {
            _selectedParticipantIndex = 0;
            if (value == null)
                _participants = GetDefaultParticipantList().ToList();
            else
                _participants = value;            
        }

    }
}