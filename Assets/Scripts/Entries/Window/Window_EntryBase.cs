using System;
using UnityEditor;
using UnityEngine;

public abstract class Window_EntryBase : EditorWindow
{

    //data variables----------------------------------------------------------------
    [SerializeField] private EntryBase entryData;
    public EntryBase EntryData
    {
        get { return entryData; }
        set { entryData = value; }
    }

 
    //UI window variables-----------------------------------------------------------


    //messages----------------------------------------------------------------------
    protected abstract void OnGUI();

    //custom methods----------------------------------------------------------------
    public virtual void Initialize(EntryBase data)
    {
        if (data == null)
            throw new ArgumentNullException();
        EntryData = data;

    }

    private void OnDestroy()
    {
        if(EntryData != null)
            if (EntryData.OnWindowClose != null)
                EntryData.OnWindowClose();
    }




}