using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Adapter_ComponentsTemplateWindow: IList<EntryComponent>
{
    [SerializeField] private Entry_ComponentsEntryTemplate _templateComponent;
    [SerializeField] private ListChangeType lastChange;

    public ListChangeType LastListChangeType
    {
        get { return lastChange; }
    }
    public List<EntryComponentTemplate> TemplatesList
    {
        get { return _templateComponent.TemplateComponents; }
    }
    public Adapter_ComponentsTemplateWindow(Entry_ComponentsEntryTemplate component)
    {
        if(component==null)
            throw new ArgumentNullException();
        _templateComponent = component;
        lastChange = ListChangeType.None;
    }


    public IEnumerator<EntryComponent> GetEnumerator()
    {
        return TemplatesList.Select(x => x.TemplateInstance).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(EntryComponent item)
    {
        TemplatesList.Add(new EntryComponentTemplate(item));
        lastChange = ListChangeType.Add;
        _templateComponent.OnTemplateChanged();
    }

    public void Clear()
    {
        TemplatesList.Clear();
        lastChange = ListChangeType.Clear;
        _templateComponent.OnTemplateChanged();

    }

    public bool Contains(EntryComponent item)
    {
        return TemplatesList.Any(x => x.TemplateInstance.Equals(item));
    }


    public void CopyTo(EntryComponent[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(EntryComponent item)
    {
        var findIndex = TemplatesList.FindIndex(x => x.TemplateInstance.Equals(item));
        if (findIndex != -1)
        {
            RemoveAt(findIndex);
            lastChange = ListChangeType.Remove;
            _templateComponent.OnTemplateChanged();
        }

        return findIndex != -1;
    }

    public int Count
    {
        get { return TemplatesList.Count; }
    }

    public bool IsReadOnly
    {
        get { return false; }
    }

    public int IndexOf(EntryComponent item)
    {
        return TemplatesList.FindIndex(x => x.TemplateInstance.Equals(item));
    }

    public void Insert(int index, EntryComponent item)
    {
        Add(item);
        TemplatesList.Insert(index, TemplatesList[TemplatesList.Count - 1]);
        TemplatesList.RemoveAt(TemplatesList.Count - 1);
    }

    public void RemoveAt(int index)
    {
        TemplatesList[index].ClearObservers();
        TemplatesList.RemoveAt(index);
        lastChange = ListChangeType.Remove;
        _templateComponent.OnTemplateChanged();

    }

    public EntryComponent this[int index]
    {
        get { return TemplatesList[index].TemplateInstance; }
        set
        {
            TemplatesList[index] = new EntryComponentTemplate(value);
            lastChange = ListChangeType.Set;
            _templateComponent.OnTemplateChanged();
        }
    }
}