using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EntryComponentTemplate 
{

    [SerializeField] private EntryComponent _templateComponent;
    [SerializeField] private List<EntryComponent> _observersList;

    public List<EntryComponent> ObserversList
    {
        get { return _observersList ?? (_observersList = new List<EntryComponent>()); }
    }
    public EntryComponent TemplateComponent
    {
        get { return _templateComponent; }
    }
    //-----------------------------------------------------------------
    public EntryComponentTemplate(Type componentType)
    {
        InitializeTemplate(componentType);
    }

    public EntryComponentTemplate(EntryComponentTemplate other)
    {
        if(other == null)
            throw new ArgumentNullException();
        _templateComponent = (EntryComponent) other.TemplateComponent.Clone();
        _observersList = _observersList.Select(x => x.Clone() as EntryComponent).ToList();
    }

    
    private void InitializeTemplate(Type componentType)
    {

        if(_templateComponent == null)
            ObserversList.Clear();

        if (_templateComponent != null)
            if(componentType != _templateComponent.GetType())
                ObserversList.Clear();
            else return;

        if (!typeof(EntryComponent).IsAssignableFrom(componentType))
            throw new ArgumentException("type must be a subclass of EntryComponent");


        _templateComponent = EntryComponent.CreateInstance(componentType);
        _templateComponent.OnEditModeModified += OnTemplateEditModeModified;

    }
    private void OnTemplateEditModeModified()
    {
        //note: can clear the _observersList, cause otherwise the _observersList elements are cloned from the template
        ObserversList.ForEach(x =>
        {
            TemplateComponent.CloneTo(x);
            x.IsInEditMode = false;
        });
    }

    //-----------------------------------------------------------------
    public void SetTemplateType(Type componentType)
    {
        InitializeTemplate(componentType);
    }
    public EntryComponent AddObserver()
    {
        var instance = (EntryComponent)TemplateComponent.Clone();
        // instance.OnEditModeModified += OnTemplateEditModeModified;
        ObserversList.Add(instance);
        return instance;
    }
    public EntryComponent this[int index]
    {
        get { return ObserversList[index]; }

    }
    //-----------------------------------------------------------------



}