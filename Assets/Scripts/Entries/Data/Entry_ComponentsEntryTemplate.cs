using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Entry_ComponentsEntryTemplate : EntryBase,ITemplate<Entry_Components>
{
    [SerializeField] private Entry_Components _templateInstance;
    [SerializeField] private List<EntryComponentTemplate> _componentTemplatesList;
    [SerializeField] private List<Entry_Components> _observers;

    //fields---------------------------------------------------------------------------------------------


    //constructors---------------------------------------------------------------------------------------

    //properties-----------------------------------------------------------------------------------------

    //methods--------------------------------------------------------------------------------------------    
    public Entry_Components TemplateInstance
    {
        get
        {
            if (_templateInstance == null)
            {
                _templateInstance = EntryBase.CreateInstance<Entry_Components>();
                _templateInstance.AddListener_OnComponentChanged(OnComponentChanged);
                _templateInstance.ShowAddButton = _templateInstance.ShowRemoveButton
                    = _templateInstance.ShowDraggableButton = _templateInstance.ShowEditModeOption = true;
            }
            return _templateInstance;
        }
    }
    public List<EntryComponentTemplate> ComponentTemplatesList
    {
        get
        {
            if (_componentTemplatesList == null)
                _componentTemplatesList = new List<EntryComponentTemplate>();
            return _componentTemplatesList;
        }
    }
    public List<Entry_Components> ObserversList
    {
        get { return _observers ?? (_observers = new List<Entry_Components>()); }
    }




    public Entry_Components AddObserver()
    {
        Entry_Components componentEntry = EntryBase.CreateInstance<Entry_Components>();

        //add observer components
        ComponentTemplatesList.ForEach(x=>componentEntry.Componets.Add(x.AddObserver()));

        //set observer settings
        componentEntry.ShowAddButton = componentEntry.ShowRemoveButton
            = componentEntry.ShowDraggableButton = componentEntry.ShowEditModeOption = false;

        //add observer to list
        ObserversList.Add(componentEntry);


        return componentEntry;
    }
    public void RemoveObserver(int index)
    {
        ScriptableObject.Destroy(ObserversList[index]);
        ObserversList.RemoveAt(index);
    }
    public void RemoveObserver(Entry_Components observer)
    {
        var index = ObserversList.IndexOf(observer);
        RemoveObserver(index);
    }
    public void ClearObservers()
    {
        ObserversList.Clear();
    }


    private void OnComponentChanged(ListChangeType listChangeType, int i1, int i2)
    {
        switch (listChangeType)
        {
            //---------------------------------------------------------------------------------------------------------------------
            case ListChangeType.Add:
                ComponentTemplatesList.Add(new EntryComponentTemplate(TemplateInstance.Componets[i1]));
                ObserversList.ForEach(x => x.Componets.Add(ComponentTemplatesList[ComponentTemplatesList.Count - 1].AddObserver()));
                TemplateInstance.Componets[i1].OnEditModeModified += () => ApplyComponentToObservers(TemplateInstance.Componets.IndexOf(TemplateInstance.Componets[i1]));
                break;
            //---------------------------------------------------------------------------------------------------------------------

            case ListChangeType.Duplicate:
                ComponentTemplatesList.Insert(i1 + 1, new EntryComponentTemplate(TemplateInstance.Componets[i1 + 1]));
                ObserversList.ForEach(x => x.Componets.Insert(i1 + 1, ComponentTemplatesList[i1 + 1].AddObserver()));
                TemplateInstance.Componets[i1].OnEditModeModified += () => ApplyComponentToObservers(TemplateInstance.Componets.IndexOf(TemplateInstance.Componets[i1]));

                break;
            //---------------------------------------------------------------------------------------------------------------------

            case ListChangeType.Remove:
                ComponentTemplatesList.RemoveAt(i1);


                if (ComponentTemplatesList.Count == 0)
                    goto case ListChangeType.Clear;


                //remove component from observers
                ObserversList.ForEach(x => x.Componets.RemoveAt(i1));

                break;

            //---------------------------------------------------------------------------------------------------------------------

            case ListChangeType.Clear:
                ComponentTemplatesList.Clear();
                ObserversList.ForEach(x => x.Componets.Clear());

                break;

            //---------------------------------------------------------------------------------------------------------------------

            case ListChangeType.Reorder:
                var temp = ComponentTemplatesList[i1];
                ComponentTemplatesList[i1] = ComponentTemplatesList[i2];
                ComponentTemplatesList[i2] = temp;

                foreach (var observer in ObserversList)
                {
                    var temp2 = observer.Componets[i1];
                    observer.Componets[i1] = observer.Componets[i2];
                    observer.Componets[i2] = temp2;
                }
                break;

            //---------------------------------------------------------------------------------------------------------------------

            case ListChangeType.None:
            case ListChangeType.DataChanged:
            default:
                break;
            //---------------------------------------------------------------------------------------------------------------------

        }





    }

    public void ApplyComponentToObservers(int valueIndex)
    {
        ComponentTemplatesList[valueIndex].ObserversList
            .ForEach(ob => ComponentTemplatesList[valueIndex].TemplateInstance.CloneTo(ob));
    }
    //methods--------------------------------------------------------------------------------------------    



    //editor methods-------------------------------------------------------------------------------------

    public override EditorWindow GetNewWindow()
    {
        var window = ScriptableObject.CreateInstance<Window_Entry_ComponentsTemplate>();
        window.Initialize(this);
        return window;
    }
    public override EditorWindow GetVisableWindow()
    {
        var window = (Window_Entry_ComponentsTemplate)EditorWindow.GetWindow(typeof(Window_Entry_ComponentsTemplate));
        window.Initialize(this);
        return window;
    }

}