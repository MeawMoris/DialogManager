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

    private Action<ListChangeType> _onComponentChanged;

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
    public Action<ListChangeType> OnComponentChanged
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

    public void AddListener_OnComponentChanged(Action<ListChangeType> listener)
    {
        OnComponentChanged += listener;
    }
    public void RemoveListener_OnComponentChanged(Action<ListChangeType> listener)
    {
        OnComponentChanged -= listener;
    }
    protected virtual void OnComponentsChanged(ListChangeType changeType)
    {
        
    }
    //editor methods-------------------------------------------------------------------------------------

    public override EditorWindow GetNewWindow()
    {
        var window = new Window_Entry_Components();
        window.Initialize(this);
        return window; 
    }
    public override EditorWindow GetVisableWindow()
    {
        var window = (Window_Entry_Components)EditorWindow.GetWindow(typeof(Window_Entry_Components));
        window.Initialize(this);
        return window; 
    }

    //---------------------------------------------------------------------------------------------------
}


public class Entry_ComponentsEntryTemplate : Entry_Components,ITemplate<Entry_Components>
{
    [SerializeField] private List<EntryComponentTemplate> _templateComponents;
    [SerializeField] private List<Entry_Components> _observers;

    private Adapter_ComponentsTemplateWindow _templatesListAdapter;

    //fields---------------------------------------------------------------------------------------------
    public List<EntryComponentTemplate> TemplateComponents
    {
        get { return _templateComponents ?? (_templateComponents = new List<EntryComponentTemplate>()); }
    }
    public Adapter_ComponentsTemplateWindow TemplatesListAdapter
    {
        get { return _templatesListAdapter ?? (_templatesListAdapter = new Adapter_ComponentsTemplateWindow(this)); }
    }
    public override IList<EntryComponent> Componets
    {
        get { return TemplatesListAdapter; }
    }


    //constructors---------------------------------------------------------------------------------------

    //properties-----------------------------------------------------------------------------------------

    //methods--------------------------------------------------------------------------------------------    
    //methods--------------------------------------------------------------------------------------------    
    public Entry_Components TemplateInstance
    {
        get { return this; }
    }
    public List<Entry_Components> ObserversList
    {
        get { return _observers ?? (_observers = new List<Entry_Components>()); }
    }
    public Entry_Components AddObserver()
    {
        Entry_Components componentEntry = new Entry_Components();
        TemplateComponents.ForEach(x=>componentEntry.Componets.Add(x.AddObserver()));
        //componentEntry.OnWindowClose += () => RemoveObserver(componentEntry);
        ObserversList.Add(componentEntry);
        return componentEntry;
    }

    public void RemoveObserver(int index)
    {
        ObserversList.RemoveAt(index);
    }
    public void RemoveObserver(Entry_Components observer)
    {
        ObserversList.Remove(observer);
    }
    public void ClearObservers()
    {
        ObserversList.Clear();
    }
    public void OnTemplateChanged()
    {
        if (ObserversList.Count == 0) return;

        switch (TemplatesListAdapter.LastListChangeType)
        {

            case ListChangeType.Add:
                ObserversList.ForEach(x=>x.Componets.Add(TemplateComponents[TemplateComponents.Count-1].AddObserver()));
                break;

            case ListChangeType.Remove:

                if(TemplateComponents.Count == 0)
                    goto case ListChangeType.Clear;
                
                //find removed component index
                int removedComponentIndex =-1;
                for (var i = 0; i < ObserversList[0].Componets.Count; i++)                   
                    if (!ObserversList[0].Componets[i].FieldName.Equals(TemplateComponents[i].TemplateInstance.FieldName))
                    { removedComponentIndex = i; break;}

                //remove component from observers
                ObserversList.ForEach(x=>x.Componets.RemoveAt(removedComponentIndex));
                break;

               
            case ListChangeType.Clear:
                ObserversList.ForEach(x => x.Componets.Clear());
                
                break;
            case ListChangeType.Set:

                //find added component index
                int changedComponentIndex = -1;
                for (var i = 0; i < TemplateComponents.Count; i++)
                    if (!ObserversList[0].Componets[i].FieldName.Equals(TemplateComponents[i].TemplateInstance.FieldName))
                    { changedComponentIndex = i; break; }

                if(changedComponentIndex == -1)
                    break;

                //insert component from observers
                ObserversList.ForEach(x => x.Componets[changedComponentIndex]= TemplateComponents[changedComponentIndex].AddObserver());

                break;


            default:
                throw new ArgumentOutOfRangeException();
        }



    }

    //editor methods-------------------------------------------------------------------------------------
    public override EditorWindow GetNewWindow()
    {
        var window = new Window_Entry_ComponentsEntryTemplate();
        window.Initialize(this);

        return window; // todo to implement, set entry limits
    }
    public override EditorWindow GetVisableWindow()
    {
        var window = (Window_Entry_Components) EditorWindow.GetWindow(typeof(Window_Entry_ComponentsEntryTemplate));
        window.Initialize(this);
        return window; // todo to implement, set entry limits
    }

}


public class Window_Entry_ComponentsEntryTemplate: Window_Entry_Components
{

}
