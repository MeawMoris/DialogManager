using System;
using System.Collections.Generic;
using Rotorz.ReorderableList;

public class ExternalListAdapter<t> : GenericListAdaptor<t>
{
    public Action<IList<t>> OnAdd;
    public Action<IList<t>,int> OnRemove;
    public Action<IList<t>, int> OnInsert;
    public Action<IList<t>, int> OnDuplicate;
    public Func<IList<t>, int, float> OnGetItemHeight;


    public ExternalListAdapter(IList<t> list, ReorderableListControl.ItemDrawer<t> itemDrawer, float itemHeight) : base(list, itemDrawer, itemHeight)
    {
    }

    public override void Add()
    {
        if(OnAdd != null)
            OnAdd(List);
    }

    public override void Remove(int index)
    {
        if (OnRemove != null)
            OnRemove(List, index);
    }

    public override void Duplicate(int index)
    {
        if (OnDuplicate != null)
            OnDuplicate(List, index);
    }

    public override void Insert(int index)
    {
        if (OnInsert != null)
            OnInsert(List, index);
    }

    public override float GetItemHeight(int index)
    {
        if (OnGetItemHeight != null)
            return OnGetItemHeight(List,index);

        return base.GetItemHeight(index);
    }
}