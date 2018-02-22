using System;
using System.Collections.Generic;
using Rotorz.ReorderableList;

public class EntryComponentTemplateListAdaptor : GenericListAdaptor<EntryComponentTemplate>
{
    private Action<IList<EntryComponentTemplate>> _onAddButtonClick;

    public EntryComponentTemplateListAdaptor(IList<EntryComponentTemplate> list, ReorderableListControl.ItemDrawer<EntryComponentTemplate> itemDrawer, Action<IList<EntryComponentTemplate>> onAddButtonClick) : base(list, itemDrawer, ReorderableListGUI.DefaultItemHeight)
    {
        if(onAddButtonClick == null)
            throw new ArgumentNullException();

        _onAddButtonClick = onAddButtonClick;
    }




    //list Methods------------------------------------------------------------------------
    public override void Add()
    {
        if (_onAddButtonClick != null)
            _onAddButtonClick.Invoke(List);

    }

    public override void Insert(int index)
    {
        Add();
        List.Insert(index, List[List.Count - 1]);
        Remove(List.Count - 1);
    }

    public override void Duplicate(int index)
    {
        List.Insert(index, null);
        List[index] = List[index + 1].Clone() as EntryComponentTemplate;

    }



    //gui Methods-------------------------------------------------------------------------

    public override float GetItemHeight(int index)
    {
        return Math.Max(ReorderableListGUI.DefaultItemHeight, List[index].TemplateInstance.GetPropertyHeight());
    }

    //------------------------------------------------------------------------------------


}