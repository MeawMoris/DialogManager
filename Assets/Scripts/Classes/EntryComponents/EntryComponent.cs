
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;


/*
todo: create dialogEntry component
todo: add the option to create a custom data group from components, and create a EntryComponent_GroupInstance component for it
todo: manifest all components
todo: save components and entries in an asset 

add support for basic fields
todo: add support for rich text field
todo?: layers and tags components

*/
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
        AssetsPath.CreateAsset(value,AssetsPath.AssetName_Components);
        value.Initialize();
        return value;
    }


    //fields--------------------------------------------------------------------------------------
    [SerializeField] private new string name;
    [SerializeField] private bool _isInEditMode;
    [SerializeField] private bool _isInitialized;
    [SerializeField] private bool _showEditFieldName;
    public Action OnEditModeModified;
    public Action OnViewModeModified;

    [SerializeField] private Entry_Components _holder;
    private bool _showFieldTypeLabel;

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
        get { return name; }
        set
        {
            if (name != value )
            {

                name = value;

                if(OnEditModeModified != null)
                    OnEditModeModified();
            }
            else name = value;
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


    public virtual string FieldTypeLabel
    {
        get { return string.Format("({0})", GetType().Name.Split('_')[1]); }
    }
    public string DefaultFieldTypeLabel
    {
        get { return string.Format("({0})", GetType().Name.Split('_')[1]); }
    }
    public virtual bool ShowFieldTypeLabel
    {
        get { return _showFieldTypeLabel; }
        set { _showFieldTypeLabel = value; }
    }

    public Entry_Components Holder
    {
        get { return _holder; }
    }

    //constructors--------------------------------------------------------------------------------
    public void Initialize(Entry_Components holder,string componentName = "Field Name")
    {
        _holder = holder;
        Initialize(componentName);
    }
    protected virtual void Initialize(string componentName = "Field Name")
    {
        FieldName = componentName;
        _isInEditMode = false;
        _isInitialized = true;
        _showEditFieldName = true;
        _showFieldTypeLabel = false;

    }

    //IClonable interface-------------------------------------------------------------------------
    public object Clone()
    {
        var value = (EntryComponent) ScriptableObject.CreateInstance(GetType());
        CloneTo(value);
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
        other._holder = _holder;
        other._showFieldTypeLabel = ShowFieldTypeLabel;


    }


    //--------------------------------------------------------------------------------------------


    //editor methods------------------------------------------------------------------------------
#if UNITY_EDITOR
    public static float SingleLineHeight
    {
        get { return EditorGUIUtility.singleLineHeight + 5; }
    }

    /// <summary>
    /// the property will be drawn based on the "IsInEditMode" property
    /// </summary>
    /// <param name="pos">the area in which to draw </param>
    public void DrawView(ref Rect pos)
    {
        if (ShowFieldTypeLabel)
        {
            pos.height = SingleLineHeight;
            var oldColor = GUI.contentColor;
            GUI.contentColor = Color.cyan;
           // DrawQuad(pos, 100, 0, 0, .2f);
            EditorGUI.LabelField(pos, FieldTypeLabel);
            GUI.contentColor = oldColor;
            pos.y += pos.height;
        }

        if (IsInEditMode)
            DrawEdit(ref pos);
        else Draw(ref pos);
    }

    /// <summary>
    /// the property will be drawn based on the "drawEditMode" argument and NOT the "IsInEditMode" property
    /// </summary>
    /// <param name="pos">the area in which to draw </param>
    /// <param name="drawEditMode">determents what layout to draw</param>
    [Obsolete("method is not implemented as it should, and will cause error, use the other method overloads.")]
    public void DrawView(ref Rect pos,bool drawEditMode)
    {//bug: fix the GetPropertyHeight method, as it calculated the height based on the IsInEditMode boolean
        if (drawEditMode)
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
        {
            FieldName = EditorGUI.DelayedTextField(pos, "Field Name", FieldName);
            pos.y += pos.height;
        }

    }

    public virtual float GetPropertyHeight()
    {
        float returnVal = 0;
        if (IsInEditMode && ShowEditFieldName)
            returnVal = SingleLineHeight;
        if (ShowFieldTypeLabel)
            returnVal += SingleLineHeight;
        return returnVal;
    }



    private Texture2D _cellBackgroundTexture;
   protected void DrawQuad(Rect position, int r, int g, int b, float a = 1)
    {
        Color color = new Color(r / 255f, g / 255f, b / 255f, a);
        if (_cellBackgroundTexture == null)
            _cellBackgroundTexture = new Texture2D(1, 1);
        _cellBackgroundTexture.SetPixel(0, 0, color);
        _cellBackgroundTexture.Apply();
        GUI.skin.box.normal.background = _cellBackgroundTexture;
        GUI.Box(position, GUIContent.none);
    }

#endif
    //--------------------------------------------------------------------------------------------
}


//---------------------------------------------------------------
//---------------------------------------------------------------