using System;
using UnityEditor;
using UnityEngine;


[Serializable]
public abstract class EntryBase : ScriptableObject
{


    public abstract EditorWindow GetNewWindow();
    public abstract EditorWindow GetVisableWindow();

    private Action _onWindowClose;


    public Action OnWindowClose
    {
        get { return _onWindowClose; }
        set { _onWindowClose = value; }
    }



}

