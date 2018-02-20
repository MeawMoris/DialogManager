using System;
using UnityEditor;
using UnityEngine;


[Serializable]
public abstract class EntryBase : ScriptableObject
{


    public abstract EditorWindow GetNewWindow();
    public abstract EditorWindow GetVisableWindow();

}

