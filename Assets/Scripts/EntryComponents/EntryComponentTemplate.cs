using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EntryComponentTemplate : ICloneable, ITemplate<EntryComponent>
{
    [SerializeField] private EntryComponent _templateComponent;
    [SerializeField] private List<EntryComponent> _observersList;
    [SerializeField] private Entry_Components _holder;

    //-----------------------------------------------------------------
    public EntryComponentTemplate(EntryComponent template)
    {
        if(template == null)
            throw new ArgumentNullException();
        _templateComponent = template;
        _holder = template.Holder;
        _templateComponent.OnEditModeModified += OnTemplateEditChanged;
    }
    public EntryComponentTemplate(Type componentType, Entry_Components holder)
    {

            _holder = holder;
            InitializeTemplate(componentType);
        

    }
    public EntryComponentTemplate(EntryComponentTemplate other)
    {
        if(other == null)
            throw new ArgumentNullException();
        _holder = other._holder;
        _templateComponent = (EntryComponent) other.TemplateInstance.Clone();
        //note: the observers list was not cloned as it will mess up the "components entry template"
       // _observersList = _observersList.Select(x => x.Clone() as EntryComponent).ToList();
    }
    public object Clone()
    {
        EntryComponentTemplate returnVal = new EntryComponentTemplate(this);
        return returnVal;
    }

    private void InitializeTemplate(Type componentType)
    {


        if (_templateComponent == null)
            ObserversList.Clear();

        if (_templateComponent != null)
            if (componentType != _templateComponent.GetType())
                ObserversList.Clear();
            else return;

        if (!typeof(EntryComponent).IsAssignableFrom(componentType))
            throw new ArgumentException("type must be a subclass of EntryComponent");


        _templateComponent = EntryComponent.CreateInstance(componentType);
        _templateComponent.Initialize(_holder);
        _templateComponent.OnEditModeModified += OnTemplateEditChanged;


    }

    //-----------------------------------------------------------------
    public void OnTemplateEditChanged()//note: on edit changed.. component value is not taken into consideration
    {
        //note: can clear the _observersList, cause otherwise the _observersList elements are cloned from the template
        ObserversList.ForEach(x =>
        {
            TemplateInstance.CloneTo(x);
            x.IsInEditMode = false;
        });
    }
    public void SetTemplateType(Type componentType)
    {
        InitializeTemplate(componentType);
    }

    //-----------------------------------------------------------------
    public List<EntryComponent> ObserversList
    {
        get { return _observersList ?? (_observersList = new List<EntryComponent>()); }
    }
    public EntryComponent TemplateInstance
    {
        get { return _templateComponent; }
    }


    public EntryComponent AddObserver()
    {
        var instance = (EntryComponent)TemplateInstance.Clone();
        ObserversList.Add(instance);

        return instance;
    }
    public void RemoveObserver(int index)
    {
        ObserversList.RemoveAt(index);
    }
    public void RemoveObserver(EntryComponent other)
    {
        ObserversList.Remove(other);
    }
    public void ClearObservers()
    {
        ObserversList.Clear();
    }
    //-----------------------------------------------------------------


}