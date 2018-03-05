using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class Entry_Components : EntryBase
{
    
    //statics--------------------------------------------------------------------------------------------
    public static string DefaultComponentName
    {
        get { return "component"; }
    }


    //fields---------------------------------------------------------------------------------------------
    [SerializeField] private List<EntryComponent> _componets;
    [SerializeField] private bool _showEditMode;
    [SerializeField] private bool _showComponentsTypeLabel;

    [SerializeField] private bool _showAddButton;
    [SerializeField] private bool _showRemoveButton;
    [SerializeField] private bool _showDraggableButton;
    [SerializeField] private bool _showEditModeOption;

    private Action<ListChangeType,int,int> _onComponentChanged;

    //constructors---------------------------------------------------------------------------------------


    //properties-----------------------------------------------------------------------------------------
    public virtual IList<EntryComponent> Componets
    {
        get
        {
            if(_componets == null)
                _componets = new List<EntryComponent>();
            return _componets;
        }
    }
    public Action<ListChangeType, int, int> OnComponentChanged
    {
        get
        {
            if (_onComponentChanged == null)
                _onComponentChanged = OnComponentsChanged;
            return _onComponentChanged;
        }
       private set { _onComponentChanged = value; }
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
    public bool ShowRemoveButton
    {
        get { return _showRemoveButton; }
        set { _showRemoveButton = value; }
    }
    public bool ShowDraggableButton
    {
        get { return _showDraggableButton; }
        set { _showDraggableButton = value; }
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



    //methods--------------------------------------------------------------------------------------------
    public void ValidateFieldName(EntryComponent component)
    {
        if(!Componets.Contains(component))
            throw new ArgumentException("component does not belong to entry");

        if (string.IsNullOrEmpty(component.FieldName))
            throw new ArgumentNullException("field new name must not be null");

        if (Componets.Any(x => x != component && x.FieldName.Equals(component.FieldName)))
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

    public void AddListener_OnComponentChanged(Action<ListChangeType, int, int> listener)
    {
        OnComponentChanged += listener;
    }
    public void RemoveListener_OnComponentChanged(Action<ListChangeType, int, int> listener)
    {
        OnComponentChanged -= listener;
    }
    protected virtual void OnComponentsChanged(ListChangeType changeType,int index1, int index2)
    {
        
    }
    //editor methods-------------------------------------------------------------------------------------

    public override EditorWindow GetNewWindow()
    {
        var window = new Window_Entry_Components();
        window.Initialize(this);
        return window; 
    }
    public override EditorWindow GetVisibleWindow()
    {
        var window = (Window_Entry_Components)EditorWindow.GetWindow(typeof(Window_Entry_Components));
        window.Initialize(this);
        return window; 
    }

    protected override void OnCreateInstance()
    {
        AssetsPath.CreateAsset(this, AssetsPath.AssetName_Entries);

    }

    //---------------------------------------------------------------------------------------------------
}