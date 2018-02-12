using System;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using UnityEditor;


//todo put this class in the Editor folder


/// <summary>
/// ReOrderable list adapter for EntryComponent
/// </summary>
/// <typeparam name="t"></typeparam>
public class EntryComponentListAdaptor<t> : GenericListAdaptor<t>
    where t : EntryComponent
{

    private readonly GenericMenu _addRightClickMenu;


    public EntryComponentListAdaptor(IList<t> list,Action<GenericMenu, IList<t>> OnAddClick, ReorderableListControl.ItemDrawer<t> itemDrawer) 
        : base(list, itemDrawer, ReorderableListGUI.DefaultItemHeight)
    {
        if (OnAddClick != null)
        {
            _addRightClickMenu = new GenericMenu();
            OnAddClick.Invoke(_addRightClickMenu,list);
        }

    }


    t CreateInstance()
    {
        return EntryComponent.CreateInstance(typeof(t)) as t;
    }



    //list Methods------------------------------------------------------------------------
    public override void Add()
    {
        if(_addRightClickMenu != null)
            _addRightClickMenu.ShowAsContext();

        else
            List.Add(CreateInstance());
    }

    public override void Insert(int index)
    {
        List.Insert(index, CreateInstance());
    }

    public override void Duplicate(int index)
    {
        Insert(index);
        List[index] = List[index + 1].Clone() as t;

    }



    //gui Methods-------------------------------------------------------------------------


    public override float GetItemHeight(int index)
    {
        return Math.Max(ReorderableListGUI.DefaultItemHeight ,List[index].GetPropertyHeight());
    }

    //------------------------------------------------------------------------------------

 
}