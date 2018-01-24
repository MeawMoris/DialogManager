using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct ParticipantSelector
{
    [SerializeField] string _selectedItemName;

    public ParticipantSelector( string participantName="")
    {
        _selectedItemName = participantName;
    }
    public DialogParticipant SelectedParticipant
    {
        get
        {
            var item = _selectedItemName;
            return DialogManager.GetInstance().GetAllParticipants().FirstOrDefault(x => x.Name.Equals(item));
        }
    }

    public string ParticipantName
    {
        get { return _selectedItemName; }
    }
}