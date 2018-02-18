using System;
using System.Collections.Generic;

[Serializable]
public class ObservedList<T> : List<T>
{
    public event Action<int> Changed;
    public event Action Updated;
    public new void Add(T item)
    {
        base.Add(item);
        if(Updated != null) Updated();
    }
    public new void Remove(T item)
    {
        base.Remove(item);
        if(Updated != null) Updated();
    }
    public new void AddRange(IEnumerable<T> collection)
    {
        base.AddRange(collection);
        if(Updated != null) Updated();
    }
    public new void RemoveRange(int index, int count)
    {
        base.RemoveRange(index, count);
        if(Updated != null) Updated();
    }
    public new void Clear()
    {
        base.Clear();
        if(Updated != null) Updated();
    }
    public new void Insert(int index, T item)
    {
        base.Insert(index, item);
        if(Updated != null) Updated();
    }
    public new void InsertRange(int index, IEnumerable<T> collection)
    {
        base.InsertRange(index, collection);
        if(Updated != null) Updated();
    }
    public new void RemoveAll(Predicate<T> match)
    {
        base.RemoveAll(match);
        if(Updated != null) Updated();
    }


    public new T this[int index]
    {
        get
        {
            return base[index];
        }
        set
        {
            base[index] = value;
            if(Changed != null)
                Changed(index);
        }
    }


}