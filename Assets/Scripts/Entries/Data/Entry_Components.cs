using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rotorz.ReorderableList;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Entry_Components : EntryBase
{
    //fields---------------------------------------------------------------------------------------------
    [SerializeField] private List<EntryComponent> _componets;
    [SerializeField] private bool _showEditMode;
    [SerializeField] private bool _showComponentsTypeLabel;
    [SerializeField] private bool _showAddButton;
    [SerializeField] private bool _showEditModeOption;

    //constructors---------------------------------------------------------------------------------------


    //properties-----------------------------------------------------------------------------------------
    public virtual List<EntryComponent> Componets
    {
        get
        {
            if(_componets == null)
                _componets = new List<EntryComponent>();
            return _componets;
        }
        private set { _componets = value; }
    }


    public bool ShowEditMode
    {
        get { return _showEditMode; }
        set { _showEditMode = value; }
    }
    public bool ShowAddButton
    {
        get { return _showAddButton; }
        set { _showAddButton = value; }
    }
    public bool ShowEditModeOption
    {
        get { return _showEditModeOption; }
        set { _showEditModeOption = value; }
    }
    public bool ShowComponentsTypeLabel
    {
        get { return _showComponentsTypeLabel; }
        set { _showComponentsTypeLabel = value; }
    }
    private string DefaultComponentName
    {
        get { return "component"; }
    }


    //methods--------------------------------------------------------------------------------------------
    public void ValidateFieldName(EntryComponent component)
    {
        if(!_componets.Contains(component))
            throw new ArgumentException("component does not belong to entry");

        if (string.IsNullOrEmpty(component.FieldName))
            throw new ArgumentNullException("field new name must not be null");

        if (_componets.Any(x => x != component && x.FieldName.Equals(component.FieldName)))
            component.FieldName = GetNextAvailableName();
    }
    public string GetNextAvailableName()
    {
        int i = 0;
        while (Componets.Any(x => x.FieldName.Equals(string.Format("{0} {1}",DefaultComponentName,i))))
        {
            i++;
        }
        return string.Format("{0} {1}", DefaultComponentName, i);
    }
    //editor methods-------------------------------------------------------------------------------------

    public override EditorWindow GetNewWindow()
    {
        var window = new Window_Entry_Components();
        window.Initialize(this);
        return window; // todo to implement
    }
    public override EditorWindow GetVisableWindow()
    {
        var window = (Window_Entry_Components)EditorWindow.GetWindow(typeof(Window_Entry_Components));
        window.Initialize(this);
        return window; // todo to implement
    }

    //---------------------------------------------------------------------------------------------------
}


public class Entry_ComponentsEntryTemplate : Entry_Components
{
   [SerializeField] private List<EntryComponentTemplate> _templateComponents;

    //fields---------------------------------------------------------------------------------------------
    public List<EntryComponentTemplate> TemplateComponents
    {
        get
        {
            if(_templateComponents == null)
                _templateComponents = new List<EntryComponentTemplate>();
            return _templateComponents;
        }
    }
    public override List<EntryComponent> Componets
    {
        get { return TemplateComponents.Select(x=>x.TemplateComponent).ToList(); }
    }

    //constructors---------------------------------------------------------------------------------------

    //properties-----------------------------------------------------------------------------------------

    //methods--------------------------------------------------------------------------------------------

    //editor methods-------------------------------------------------------------------------------------
    public override EditorWindow GetNewWindow()
    {
        var window = new Window_Entry_ComponentsEntryTemplate();
        window.Initialize(this);
        return window; // todo to implement
    }
    public override EditorWindow GetVisableWindow()
    {
        var window = (Window_Entry_Components)EditorWindow.GetWindow(typeof(Window_Entry_ComponentsEntryTemplate));
        window.Initialize(this);
        return window; // todo to implement
    }

}

public class Window_Entry_ComponentsEntryTemplate: Window_Entry_Components
{
    protected new EntryComponentListAdaptor<EntryComponent> _componentsListadapter;

}
