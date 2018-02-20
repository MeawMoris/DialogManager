using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class EntryComponent_Collection : EntryComponent_SelectTypeBase
{

    static readonly List<EntryComponent> EmptyList = new List<EntryComponent>();
    //-------------------------------------------------------------------------------------------------------------
    private EntryComponentTemplate _template;
    private ReorderableList _reorderableList;

    //-------------------------------------------------------------------------------------------------------------
    public override object Value
    {
        get { return (_template != null ? _template.ObserversList : EmptyList); }
        set { }
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
    public List<EntryComponent> ListOptions
    {
        get { return (_template != null ? _template.ObserversList : EmptyList); }
    }

    public override string FieldTypeLabel
    {
        get
        {
            return string.Format("{0} {1}", DefaultFieldTypeLabel,
                (SelectedType == null ? "" : string.Format("- {0}", SelectedType.Name.Split('_')[1])));
        }
    }

    //-------------------------------------------------------------------------------------------------------------
    protected override void DrawEdit(ref Rect pos)
    {
        base.DrawEdit(ref pos);
        if (SelectedType != null)
        {
            if(_template == null)
                _template = new EntryComponentTemplate(SelectedType,Holder);
            _template.SetTemplateType(SelectedType);
            _template.TemplateComponent.IsInEditMode = true;
            _template.TemplateComponent.ShowEditFieldName = false;
            _template.TemplateComponent.ShowFieldTypeLabel = false;
            pos.height = _template.TemplateComponent.GetPropertyHeight();
            DrawQuad(pos, 100, 0, 255, .2f);
            _template.TemplateComponent.DrawView(ref pos);
        }
    }

    protected override void DrawObjectField(ref Rect pos)
    {

        if (_reorderableList == null)       
            InitializeReorderableList();

        pos.y += 3;
        _reorderableList.DoList(pos);

    }
    public override float GetPropertyHeight()
    {
        float addedHeight = 0;
        if (!IsInEditMode && SelectedType != null && _reorderableList != null)
            addedHeight = _reorderableList.GetHeight() + 5;
        if (IsInEditMode && SelectedType != null && _template != null)
            addedHeight = _template.TemplateComponent.GetPropertyHeight();

        return base.GetPropertyHeight() + addedHeight;
    }

    public override void CloneTo(EntryComponent other)
    {
        base.CloneTo(other);
        ((EntryComponent_Collection)other)._template = new EntryComponentTemplate(_template);
    }
    //-------------------------------------------------------------------------------------------------------------


    private void InitializeReorderableList()
    {
        _reorderableList = new ReorderableList(_template.ObserversList, SelectedType,true,false,true,false);
        _reorderableList.onAddCallback += OnAddComponentClick;
        _reorderableList.drawElementCallback += DrawElementCallback;
        _reorderableList.elementHeightCallback += ElementHeightCallback;
        _reorderableList.drawElementBackgroundCallback += DrawElementBackgroundCallback;
        _reorderableList.drawHeaderCallback+= DrawHeaderCallback;
    }

    private void DrawHeaderCallback(Rect rect)
    {
        EditorGUI.LabelField(rect,FieldName);
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
        entryComponent.FieldName =string.Format("index {0}", index);
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
        DrawQuad(rect, 100, 0, 255,.2f);
    }


    private void OnAddComponentClick(ReorderableList list)
    {
        if (_template != null)
        {
            _template.AddObserver();
            _template[_template.ObserversList.Count - 1].IsInEditMode = false;
            _template[_template.ObserversList.Count - 1].ShowFieldTypeLabel = false;
        }
       
    } 
    private float ElementHeightCallback(int index)
    {
        if (index >= _reorderableList.list.Count || index < 0)
            return 0;
        return ((EntryComponent)_reorderableList.list[index]).GetPropertyHeight();
    }
    //-------------------------------------------------------------------------------------------------------------
}