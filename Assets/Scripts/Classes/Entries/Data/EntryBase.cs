using System;
using UnityEditor;
using UnityEngine;


[Serializable]
public abstract class EntryBase : ScriptableObject
{

    [SerializeField] public string _name;
    public new string name
    {
        get { return _name; }
        set { _name = value; }
    }
    public string EntryName
    {
        get { return _name; }
        set { _name = value; }
    }




    public abstract EditorWindow GetNewWindow();
    public abstract EditorWindow GetVisibleWindow();

    private Action _onWindowClose;
    public Action OnWindowClose
    {
        get
        {
            return _onWindowClose;
        }
        set { _onWindowClose = value; }
    }

    protected abstract void OnCreateInstance();

    public new static  T CreateInstance<T>() where T : EntryBase
    {
        return (T) CreateInstance(typeof(T));
    }
    public new static UnityEngine.Object CreateInstance(Type type) 
    {
        if( !typeof(EntryBase).IsAssignableFrom(type))
            throw new ArgumentException();
        var returnVal = (EntryBase)ScriptableObject.CreateInstance(type);
        returnVal.OnCreateInstance();
        return returnVal;
    }

}

