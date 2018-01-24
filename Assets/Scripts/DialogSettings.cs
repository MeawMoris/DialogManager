using System;
using UnityEngine;

[Serializable]
public struct DialogSettings
{

    [SerializeField] private bool _multipleSimultaneousDialogs;
    [SerializeField] private bool _showParticimantNames;
    [SerializeField] private bool _showParticipantImages;
    [SerializeField] private bool _autoPlay;


    public bool MultipleSimultaneousDialogs
    {
        get { return _multipleSimultaneousDialogs; }
        set { _multipleSimultaneousDialogs = value; }
    }
    public bool ShowParticipantImages
    {
        get { return _showParticipantImages; }
        set { _showParticipantImages = value; }
    }
    public bool ShowParticimantNames
    {
        get { return _showParticimantNames; }
        set { _showParticimantNames = value; }
    }
    public bool AutoPlay
    {
        get { return _autoPlay; }
        set { _autoPlay = value; }
    }
    //todo Calculate autoplay time based on text in slide
    //todo set manual autoplay time
    //todo set dialog display speed (instance, fast, slow, normal)
}