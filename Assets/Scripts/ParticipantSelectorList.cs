using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
struct ParticipantSelectorInfo
{
    [SerializeField] string participantName;
    [SerializeField] int participantPopupIndex;

    public string ParticipantName
    {
        get { return participantName; }
    }
}

[Serializable]
public class ParticipantSelectorList:IList<string>
{
    [SerializeField] private List<ParticipantSelectorInfo> _participantNames;

    List<string> GetNameList()
    {
        return _participantNames.Select(x => x.ParticipantName).ToList();
    }


    public IEnumerator<string> GetEnumerator()
    {
        return GetNameList().GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)GetNameList()).GetEnumerator();
    }
    public void Add(string item)
    {
        GetNameList().Add(item);
    }
    public void Clear()
    {
        GetNameList().Clear();
    }
    public bool Contains(string item)
    {
        return GetNameList().Contains(item);
    }
    public void CopyTo(string[] array, int arrayIndex)
    {
        GetNameList().CopyTo(array, arrayIndex);
    }
    public bool Remove(string item)
    {
        return GetNameList().Remove(item);
    }
    public int Count
    {
        get { return GetNameList().Count; }
    }
    public bool IsReadOnly
    {
        get { return false; }
    }
    public int IndexOf(string item)
    {
        return GetNameList().IndexOf(item);
    }
    public void Insert(int index, string item)
    {
        GetNameList().Insert(index, item);
    }
    public void RemoveAt(int index)
    {
        GetNameList().RemoveAt(index);
    }
    public string this[int index]
    {
        get { return GetNameList()[index]; }
        set { GetNameList()[index] = value; }
    }
}