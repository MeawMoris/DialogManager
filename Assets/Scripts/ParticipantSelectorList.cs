using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ParticipantSelectorList:IList<ParticipantSelector>
{
    [SerializeField] private List<ParticipantSelector> _selectors;



    public IEnumerator<ParticipantSelector> GetEnumerator()
    {
        return _selectors.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable) _selectors).GetEnumerator();
    }
    public void Add(ParticipantSelector item)
    {
        _selectors.Add(item);
    }
    public void Clear()
    {
        _selectors.Clear();
    }
    public bool Contains(ParticipantSelector item)
    {
        return _selectors.Contains(item);
    }
    public void CopyTo(ParticipantSelector[] array, int arrayIndex)
    {
        _selectors.CopyTo(array, arrayIndex);
    }
    public bool Remove(ParticipantSelector item)
    {
        return _selectors.Remove(item);
    }
    public int Count
    {
        get { return _selectors.Count; }
    }
    public bool IsReadOnly
    {
        get { return false; }
    }
    public int IndexOf(ParticipantSelector item)
    {
        return _selectors.IndexOf(item);
    }
    public void Insert(int index, ParticipantSelector item)
    {
        _selectors.Insert(index, item);
    }
    public void RemoveAt(int index)
    {
        _selectors.RemoveAt(index);
    }
    public ParticipantSelector this[int index]
    {
        get { return _selectors[index]; }
        set { _selectors[index] = value; }
    }
}