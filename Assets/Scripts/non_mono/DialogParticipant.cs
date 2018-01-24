using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogParticipant:UnityEngine.Object
{

    public const string AnonymousName = "Anonymous";



    [SerializeField] public string _name;
    [SerializeField] public List<Sprite> _sprites = new List<Sprite>();
    [SerializeField] public Sprite _defaultSprite;

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }
    public List<Sprite> Sprites
    {
        get { return _sprites; }
        set { _sprites = value; }
    }
    public Sprite DefaultSprite
    {
        get { return _defaultSprite; }
        set { _defaultSprite = value; }
    }




}


