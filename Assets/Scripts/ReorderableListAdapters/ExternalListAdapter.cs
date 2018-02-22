using System;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using UnityEngine;

public class ExternalListAdapter<t> : GenericListAdaptor<t>
{
    public Action<IList<t>> OnAdd;
    public Action<IList<t>,int> OnRemove;
    public Action<IList<t>, int> OnInsert;
    public Action<IList<t>, int> OnDuplicate;
    public Action<IList<t>, int,int> OnMove;


    public ReorderableListControl.ItemDrawer<t> OnItemDraw;
    public ReorderableListControl.ItemDrawer<t> OnItemDrawBackground;
    public Func<IList<t>, int, float> OnGetItemHeight;


    public ExternalListAdapter(IList<t> list, ReorderableListControl.ItemDrawer<t> itemDrawer) : base(list, null, ReorderableListGUI.DefaultItemHeight)
    {
        OnItemDraw = itemDrawer;
    }

    public override void Add()
    {
        if(OnAdd != null)
            OnAdd(List);
        else base.Add();
    }

    public override void Remove(int index)
    {
        if (OnRemove != null)
            OnRemove(List, index);
        else base.Remove(index);

    }

    public override void Duplicate(int index)
    {
        if (OnDuplicate != null)
            OnDuplicate(List, index);
        else base.Duplicate(index);
    }

    public override void Insert(int index)
    {
        if (OnInsert != null)
            OnInsert(List, index);

        else base.Insert(index);

    }

    public override void Move(int sourceIndex, int destIndex)
    {
        if(OnMove != null)
            OnMove(List, sourceIndex, destIndex);
        else base.Move(sourceIndex,destIndex);
    }

    public override void DrawItem(Rect position, int index)
    {
        OnItemDraw(position, List[index]);
    }

    public override void DrawItemBackground(Rect position, int index)
    {
        if(OnItemDrawBackground != null)
            OnItemDrawBackground(position, List[index]);
        else base.DrawItemBackground(position,index);
    }

    public override float GetItemHeight(int index)
    {
        if (OnGetItemHeight != null)
            return OnGetItemHeight(List,index);

        return base.GetItemHeight(index);
    }
}