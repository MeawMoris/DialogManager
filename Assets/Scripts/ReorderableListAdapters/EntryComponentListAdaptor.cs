using System;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using UnityEditor;





/// <summary>
/// ReOrderable list adapter for EntryComponent
/// </summary>
/// <typeparam name="t"></typeparam>
public class EntryComponentListAdaptor<t> : GenericListAdaptor<t>
    where t : EntryComponent
{
    private Action<IList<t>> _onAddButtonClick;

    public EntryComponentListAdaptor(IList<t> list, ReorderableListControl.ItemDrawer<t> itemDrawer, Action<IList<t>> onAddButtonClick) : base(list, itemDrawer, ReorderableListGUI.DefaultItemHeight)
    {
        _onAddButtonClick = onAddButtonClick;
    }



    t CreateInstance()
    {
        return EntryComponent.CreateInstance(typeof(t)) as t;
    }



    //list Methods------------------------------------------------------------------------
    public override void Add()
    {
        if (_onAddButtonClick != null)
            _onAddButtonClick.Invoke(List);

        else
            List.Add(CreateInstance());
    }

    public override void Insert(int index)
    {
        Add();
        var newItem = List[List.Count - 1];
        List[List.Count - 1] = List[index];
        List[index] = newItem;
    }

    public override void Duplicate(int index)
    {
        List.Insert(index,null);
        List[index] = List[index + 1].Clone() as t;

    }



    //gui Methods-------------------------------------------------------------------------

    public override float GetItemHeight(int index)
    {
        return Math.Max(ReorderableListGUI.DefaultItemHeight, List[index].GetPropertyHeight());
    }

    //------------------------------------------------------------------------------------



}