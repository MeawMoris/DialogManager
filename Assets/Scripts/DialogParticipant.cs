using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogParticipant
{





    [SerializeField]  string _name;
    [SerializeField]  List<Sprite> _sprites = new List<Sprite>();
    [SerializeField]  int _defaultSpriteIndex;



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
        get
        {
            if (_sprites.Count == 0)
                return null;
            if (_defaultSpriteIndex >= _sprites.Count)
                _defaultSpriteIndex = 0;

            return _sprites[DefaultSpriteIndex];
        }
    }
    public int DefaultSpriteIndex
    {
        get
        {
            if (_sprites.Count == 0)
                _defaultSpriteIndex = -1;
            return _defaultSpriteIndex;
        }
        set
        {
            if(value>=0 && value<_sprites.Count)
                _defaultSpriteIndex = value;
        }
    }
}