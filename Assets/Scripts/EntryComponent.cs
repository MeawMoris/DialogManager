
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rotorz.ReorderableList;
using UnityEditorInternal;

//todo: Separate editor script from non-editor
//todo: create dialogEntry component
//todo: create list component
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

    //properties----------------------------------------------------------------------------------
    public string FieldName
    {
        get { return _fieldName; }
        set { _fieldName = value; }
    }
    public bool IsInEditMode
    {
        get { return _isInEditMode; }
        set { _isInEditMode = value; }
    }

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


    //--------------------------------------------------------------------------------------------


    //editor methods------------------------------------------------------------------------------
#if UNITY_EDITOR
    public static float SingleLineHeight
    {
        get { return ReorderableListGUI.DefaultItemHeight; }
    }

    public void DrawView(ref Rect pos)
    {
        if (IsInEditMode)
            DrawEdit(ref pos);
        else Draw(ref pos);
    }
    protected virtual void Draw(ref Rect pos)
    {

    }
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



public class EntryComponent_Collection : EntryComponent_SelectTypeBase
{

    //-------------------------------------------------------------------------------------------------------------
    private ReorderableList _reorderableList;
    private List<EntryComponent> _componetnsList;
   [SerializeField] private EntryComponent _template;

    //-------------------------------------------------------------------------------------------------------------
    public override object Value
    {
        get { return _componetnsList; }
        set { _componetnsList = (List<EntryComponent>) value; }
    }
    protected override string FieldTypeFieldString
    {
        get { return "Collection Type"; }
    }
    public override List<Type> GetAvailableTypes()
    {
        return (from t in Assembly.GetExecutingAssembly().GetTypes()
           where t.IsClass && t.IsPublic && !t.IsAbstract 
           && typeof(EntryComponent).IsAssignableFrom(t) 
           && !t.IsAssignableFrom(typeof(EntryComponent_Collection))
                select t).ToList();
    }
    protected override bool ShowSearchField
    {
        get { return false; }
    }

//-------------------------------------------------------------------------------------------------------------

    protected override void DrawEdit(ref Rect pos)
    {
        base.DrawEdit(ref pos);
        if (SelectedType != null)
        {

            UpdateTemplate();
            _template.IsInEditMode = true;
            _template.DrawView(ref pos);
        }
    }  
    protected override void DrawObjectField(ref Rect pos)
    {
        if(_componetnsList == null)
            _componetnsList = new List<EntryComponent>();

        if (_reorderableList == null)       
            InitializeReorderableList();

        UpdateTemplate();


        //if the type was changed and current cells are of incomputable types
        if(_componetnsList.Count != 0 && _template != null 
           && _template is EntryComponent_SelectTypeBase
            && !((EntryComponent_SelectTypeBase)_template).SelectedType.IsAssignableFrom(((EntryComponent_SelectTypeBase)_componetnsList[0]).SelectedType))
            _componetnsList.Clear();


        _reorderableList.DoList(pos);

    }
    public override float GetPropertyHeight()
    {
        float addedHeight = 0;
        if (!IsInEditMode && SelectedType != null && _reorderableList != null)
            addedHeight = _reorderableList.GetHeight() + 5;
        if (IsInEditMode && SelectedType != null && _template != null)
            addedHeight = _template.GetPropertyHeight();

        return base.GetPropertyHeight() + addedHeight;
    }

    void UpdateTemplate()
    {
        if (_template == null)
        {
            _template = CreateInstance(SelectedType);
            _template.ShowEditFieldName = false;
        }

        else if (!SelectedType.IsInstanceOfType(_template))
        {
            _template = CreateInstance(SelectedType);
            _template.ShowEditFieldName = false;

        }
    }


    //-------------------------------------------------------------------------------------------------------------

    private void InitializeReorderableList()
    {
        _reorderableList = new ReorderableList(_componetnsList, SelectedType);
        _reorderableList.onAddCallback += OnAddComponentClick;
        _reorderableList.drawElementCallback += DrawElementCallback;
        _reorderableList.elementHeightCallback += ElementHeightCallback;
        _reorderableList.drawElementBackgroundCallback += DrawElementBackgroundCallback;
        _reorderableList.drawHeaderCallback += DrawHeaderCallback;
    }


    private void DrawHeaderCallback(Rect rect)
    {
        //throw new NotImplementedException();
        EditorGUI.LabelField(rect,string.Format("List<{0}> - {1}", SelectedType.Name, 
            (_template!=null&& _template is EntryComponent_SelectTypeBase)?((EntryComponent_SelectTypeBase) _template).SelectedType.Name:""));
        
    }
    private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        if (index >= _reorderableList.list.Count || index < 0)
            return;

        //set field width
        var pos = rect;
        pos.width -= EditorGUIUtility.singleLineHeight;

        //draw field
        var entryComponent = ((EntryComponent) _reorderableList.list[index]);
        entryComponent.FieldName = index.ToString();
        entryComponent.DrawView(ref pos);

        //calculate remove button pos
        pos.height = pos.width = EditorGUIUtility.singleLineHeight;
        pos.x = rect.x + rect.width - EditorGUIUtility.singleLineHeight;
        pos.y = rect.y + entryComponent.GetPropertyHeight() / 2 - pos.height / 2;

        //on remove button pressed
        if (GUI.Button(pos, "-"))
        {
            _reorderableList.list.RemoveAt(index);

        }
        
    }
    private void DrawElementBackgroundCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        DrawQuad(rect, 0, 38, 255,.2f);
    }
    void DrawQuad(Rect position, int r, int g, int b, float a = 1)
    {
        Color color = new Color(r / 255f, g / 255f, b / 255f, a);
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        GUI.skin.box.normal.background = texture;
        GUI.Box(position, GUIContent.none);
    }
    private void OnAddComponentClick(ReorderableList list)
    {
        if (_template != null)
        {
            EntryComponent item = (EntryComponent) _template.Clone();
            //EntryComponent item = (EntryComponent) CreateInstance(_template.GetType());
            item.IsInEditMode = false;
            item.FieldName = list.count.ToString();
            list.list.Add(item);

        }


    } //todo implement
    private float ElementHeightCallback(int index)
    {
        if (index >= _reorderableList.list.Count || index < 0)
            return 0;
        return ((EntryComponent)_reorderableList.list[index]).GetPropertyHeight();
    }
    //-------------------------------------------------------------------------------------------------------------
}