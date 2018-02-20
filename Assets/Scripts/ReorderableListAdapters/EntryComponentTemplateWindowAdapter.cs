using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EntryComponentTemplateWindowAdapter: IList<EntryComponent>
{
    [SerializeField] private List<EntryComponentTemplate> _templatesList;


    public List<EntryComponentTemplate> TemplatesList
    {
        get { return _templatesList; }
    }
    public EntryComponentTemplateWindowAdapter(List<EntryComponentTemplate> templatesList)
    {
        _templatesList = templatesList;
    }


    public IEnumerator<EntryComponent> GetEnumerator()
    {
        return TemplatesList.Select(x => x.TemplateComponent).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(EntryComponent item)
    {
        TemplatesList.Add(new EntryComponentTemplate(item));
    }

    public void Clear()
    {
        //todo notify entry Observers change
        TemplatesList.Clear();
        throw new NotImplementedException();

    }

    public bool Contains(EntryComponent item)
    {
        return _templatesList.Any(x => x.TemplateComponent.Equals(item));
    }

    public void CopyTo(EntryComponent[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(EntryComponent item)
    {
        //todo notify entry Observers change
        var findIndex = TemplatesList.FindIndex(x => x.TemplateComponent.Equals(item));
        if (findIndex != -1)
            RemoveAt(findIndex);
        return findIndex != -1;
    }

    public int Count
    {
        get { return _templatesList.Count; }
    }

    public bool IsReadOnly
    {
        get { return false; }
    }

    public int IndexOf(EntryComponent item)
    {
        return TemplatesList.FindIndex(x => x.TemplateComponent.Equals(item));
    }

    public void Insert(int index, EntryComponent item)
    {
        TemplatesList.Insert(index,new EntryComponentTemplate(item));
    }

    public void RemoveAt(int index)
    {
        //todo notify entry Observers change
        TemplatesList.RemoveAt(index);           
    }

    public EntryComponent this[int index]
    {
        get { return TemplatesList[index].TemplateComponent; }
        set
        {
            //todo notify entry Observers change
            TemplatesList[index] = new EntryComponentTemplate(value);
        }
    }
}