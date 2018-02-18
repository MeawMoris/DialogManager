
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using UnityEngine;
using System.Collections.ObjectModel;
using Rotorz.ReorderableList;

//todo: Separate editor script from non-editor
//todo: create dialogEntry component
//todo: add the option to create a custom data class from components, and create a EntryComponent_CustomData component for it
//---------------------------------------------------------------
[Serializable]
public abstract class EntryComponent:UnityEngine.ScriptableObject,ICloneable
{
    //statics-------------------------------------------------------------------------------------
    public new static EntryComponent CreateInstance<t>() where t : EntryComponent
    {
        return CreateInstance(typeof(t));
    }
    public new static EntryComponent CreateInstance(Type t)
    {
        if (t.IsAbstract || t.IsNotPublic)
            return null;

        var value = (EntryComponent)ScriptableObject.CreateInstance(t);
        value.Initialize();
        return value;
    }


    //fields--------------------------------------------------------------------------------------
    [SerializeField] private string _fieldName;
    [SerializeField] private bool _isInEditMode;
    [SerializeField] private bool _isInitialized;
    [SerializeField] private bool _showEditFieldName;
    public Action OnEditModeModified;
    public Action OnViewModeModified;
    

    //properties----------------------------------------------------------------------------------

    /// <summary>
    /// whether to draw the edit layout or the view layout
    /// </summary>
    public virtual bool IsInEditMode
    {
        get { return _isInEditMode; }
        set { _isInEditMode = value; }
    }

    public string FieldName
    {
        get { return _fieldName; }
        set
        {
            if (_fieldName != value && OnEditModeModified != null)
            {
                _fieldName = value;
                OnEditModeModified();
            }
            else _fieldName = value;
        }
    }

    /// <summary>
    /// whether to allow the user to change the component's name in edit mode or not/
    /// if false the "field name" field will not be shown in edit mode
    /// </summary>
    public virtual bool ShowEditFieldName
    {
        get { return _showEditFieldName; }
        set { _showEditFieldName = value; }
    }

    //constructors--------------------------------------------------------------------------------
    public virtual void Initialize(string componentName = "Field Name")
    {
        FieldName = componentName;
        _isInEditMode = false;
        _isInitialized = true;
        _showEditFieldName = true;
    }


    //IClonable interface-------------------------------------------------------------------------
    public virtual object Clone()
    {
        var value = (EntryComponent) ScriptableObject.CreateInstance(GetType());
        value.Initialize(FieldName);
        value._showEditFieldName = _showEditFieldName;
        return value;
    }
    public virtual void CloneTo(EntryComponent other)
    {
        if(other == null)
            throw new ArgumentNullException();

        other.FieldName = FieldName;
        other._showEditFieldName = _showEditFieldName;
        other._isInEditMode = _isInEditMode;
        other._isInitialized = _isInitialized;
    }


    //--------------------------------------------------------------------------------------------


    //editor methods------------------------------------------------------------------------------
#if UNITY_EDITOR
    public static float SingleLineHeight
    {
        get { return ReorderableListGUI.DefaultItemHeight; }
    }

    /// <summary>
    /// the property will be drawn based on the "IsInEditMode" property
    /// </summary>
    /// <param name="pos">the area in which to draw </param>
    public void DrawView(ref Rect pos)
    {
        if (IsInEditMode)
            DrawEdit(ref pos);
        else Draw(ref pos);
    }
   
    /// <summary>
    /// the property will be drawn based on the "drawEditMode" argument and NOT the "IsInEditMode" property
    /// </summary>
    /// <param name="pos">the area in which to draw </param>
    /// <param name="drawEditMode">determents what layout to draw</param>
    public void DrawView(ref Rect pos,bool drawEditMode)
    {
        if (IsInEditMode)
            DrawEdit(ref pos);
        else Draw(ref pos);
    }

    protected abstract void Draw(ref Rect pos);
    protected virtual void DrawEdit(ref Rect pos)
    {
        if (!_isInitialized)
            Initialize(FieldName);

        pos.height = SingleLineHeight;
        if (IsInEditMode && ShowEditFieldName)
            FieldName = EditorGUI.TextField(pos, "Field Name", FieldName);

    }

    public virtual float GetPropertyHeight()
    {
        return IsInEditMode && ShowEditFieldName ? SingleLineHeight : 0;
    }
#endif
    //--------------------------------------------------------------------------------------------
}
//---------------------------------------------------------------
//---------------------------------------------------------------