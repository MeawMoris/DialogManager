using System;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using UnityEditor;
using UnityEditorInternal;
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



public class ExternalReorderableListAdapter<t>
{
    public delegate void ElementDrawCallbackDelegate(IList<t> list ,Rect rect, int index, bool isActive, bool isFocused);
    protected const float SingleLineHeight = 20;

    //--------------------------------------------------------------------------
    private ReorderableList _listViewer;
    private IList<t> _list;
    private int _reorderInitialIndex = 0;
    private bool showRemoveButtonNextElement ;
    private bool highlightSelected ;
    //events--------------------------------------------------------------------
    public Action<IList<t>> CallBack_List_OnAdd;
    public ReorderableList.AddDropdownCallbackDelegate CallBack_List_onAddDropdownCallback;

    public Action<IList<t>, int> CallBack_List_OnRemove;
    public Action<IList<t>, int> CallBack_List_OnInsert;
    public Action<IList<t>, int> CallBack_List_OnDuplicate;
    public Action<IList<t>, int, int> CallBack_List_OnReorder;//on move
    //public ReorderableList.ReorderCallbackDelegate CallBack_List_Reorder;


    public ElementDrawCallbackDelegate Callback_Draw_Element;
    public ElementDrawCallbackDelegate Callback_Draw_ElementBackground;

    public ReorderableList.FooterCallbackDelegate Callback_Draw_Footer
    {
        get { return _listViewer.drawFooterCallback; }
        set { _listViewer.drawFooterCallback = value; }
    }
    public ReorderableList.HeaderCallbackDelegate Callback_Draw_Header
    {
        get { return _listViewer.drawHeaderCallback; }
        set { _listViewer.drawHeaderCallback = value; }
    }

    public Func<IList<t>,int,float> Callback_Draw_ElementHeight;


    public ReorderableList.CanAddCallbackDelegate CallBack_Setting_CanAdd
    {
        get { return _listViewer.onCanAddCallback; }
        set { _listViewer.onCanAddCallback = value; }
    }
    public ReorderableList.CanRemoveCallbackDelegate CallBack_Setting_CanRemove
    {
        get { return _listViewer.onCanRemoveCallback; }
        set { _listViewer.onCanRemoveCallback = value; }
    }
    public ReorderableList.SelectCallbackDelegate CallBack_Setting_OnMouseUp
    {
        get { return _listViewer.onMouseUpCallback; }
        set { _listViewer.onMouseUpCallback = value; }
    }
    public ReorderableList.ChangedCallbackDelegate CallBack_Setting_OnChanged
    {
        get { return _listViewer.onChangedCallback; }
        set { _listViewer.onChangedCallback = value; }
    }
    public ReorderableList.SelectCallbackDelegate CallBack_Setting_OnSelect
    {
        get { return _listViewer.onSelectCallback; }
        set { _listViewer.onSelectCallback = value; }
    }

    //-------------------------------------------------------------------------

    public ExternalReorderableListAdapter(IList<t> list)
    { 
        _list = list;
        _listViewer = new ReorderableList((IList)_list, typeof(t));


        Property_Show_HighlighSelected = true;
        Property_Show_RemoveButton = true;
        _listViewer.displayRemove = false;
        Property_Height_Header = SingleLineHeight;
        Property_Height_Elemet = SingleLineHeight;

        //add index
        _listViewer.onAddCallback += (x) => Add();
        //remove index
        _listViewer.onRemoveCallback += reorderableList => Remove(reorderableList.index);

        //reorder items
        _listViewer.onSelectCallback += reorderableList =>
        {
            _reorderInitialIndex = reorderableList.index;
        };
        _listViewer.onReorderCallback += reorderableList => Move(_reorderInitialIndex,reorderableList.index);

        //rightClick item
        _listViewer.onMouseUpCallback += OnMouseUpCallback;


        //draw events
        _listViewer.drawElementCallback += DrawElementCallback;
        _listViewer.drawElementBackgroundCallback += DrawElementBackgroundCallback;
        _listViewer.elementHeightCallback += GetItemHeight;

        //draw canAdd and cannRemove
        //_listViewer.onCanAddCallback += ca;

    }



    protected void Add()
    {
        if (CallBack_List_OnAdd != null)
            CallBack_List_OnAdd(_list);
    }
    protected  void Remove(int index)
    {


        if (CallBack_List_OnRemove != null)
            CallBack_List_OnRemove(_list, index);

    }
    protected void Duplicate(int index)
    {
        if (CallBack_List_OnDuplicate != null)
            CallBack_List_OnDuplicate(_list, index);
    }
    protected void Insert(int index)
    {
        if (CallBack_List_OnInsert != null)
            CallBack_List_OnInsert(_list, index);
    }
    protected void Move(int sourceIndex, int destIndex)
    {
        if (CallBack_List_OnReorder != null)
            CallBack_List_OnReorder(_list, sourceIndex, destIndex);
    }


    /*
     todo: implement draw item 
     todo: implement draw item remove button
     todo: implement draw item background
     todo: implement add display remove default property
     todo: implement add display remove next element property
     todo: implement add drop-down event
     todo: implement add onChanged event
         */
    private void DrawElementBackgroundCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        if (index > _list.Count)
            return;

        var elementDrawPos = (rect.position);
        var ElementSize = (rect.size);
        var mousePos = Event.current.mousePosition;
        if (Event.current.button == 1)

            if (mousePos.x > elementDrawPos.x && mousePos.x < elementDrawPos.x + ElementSize.x
                && mousePos.y > elementDrawPos.y && mousePos.y < elementDrawPos.y + ElementSize.y)
            {

                GenericMenu options = new GenericMenu();
                options.AddItem(new GUIContent("Insert Above"), false, () => Insert(index));
                options.AddItem(new GUIContent("Insert Below"), false, () => Insert(index + 1));
                options.AddSeparator("");
                options.AddItem(new GUIContent("Duplicate"), false, () => Duplicate(index));
                _listViewer.ReleaseKeyboardFocus();
                //_listViewer.GrabKeyboardFocus();


                options.ShowAsContext();
                //  isFocused = true;
            }

        if (Callback_Draw_ElementBackground != null)
            Callback_Draw_ElementBackground(_list,rect, index, isActive, isFocused);


        if (isFocused && highlightSelected)
            DrawQuad(rect, 0, 50, 186, .3f);
    }
    private void DrawElementCallback(Rect pos, int index, bool isActive, bool isFocused)
    {
        if(index>_list.Count)
            return;

        var rect = pos ;
        var canRemove =  CallBack_Setting_CanRemove(_listViewer);

        if (showRemoveButtonNextElement && canRemove)
            rect.width -= SingleLineHeight;
        if (Callback_Draw_Element != null)
            Callback_Draw_Element(_list, rect, index, isActive, isFocused);

        if (showRemoveButtonNextElement && canRemove)
        {
            //calculate remove button pos
            rect.height = rect.width = SingleLineHeight;
            rect.x = pos.x + pos.width - SingleLineHeight;
            rect.y = pos.y + GetItemHeight(index) / 2 - rect.height / 2;

            //on remove button pressed

            if (GUI.Button(rect,ReorderableList.defaultBehaviours.iconToolbarMinus, ReorderableList.defaultBehaviours.preButton))
                Remove(index);
        }


    }
    protected float GetItemHeight(int index)
    {
        if (index > _list.Count)
            return _listViewer.elementHeight;

        if (Callback_Draw_ElementHeight != null)
            return Callback_Draw_ElementHeight(_list, index);

        return _listViewer.elementHeight;
    }


    private void OnMouseUpCallback(ReorderableList list)
    {
        if (Event.current.button == 1)
        {
            GenericMenu options = new GenericMenu();
            options.AddItem(new GUIContent("Insert Above"), false, ()=> Insert(list.index));
            options.AddItem(new GUIContent("Insert Below"), false, ()=> Insert(list.index+1));
            options.AddSeparator("");
            options.ShowAsContext();
        }

    }


    private Texture2D _cellBackgroundTexture;
    protected void DrawQuad(Rect position, int r, int g, int b, float a = 1)
    {
        Color color = new Color(r / 255f, g / 255f, b / 255f, a);
        if (_cellBackgroundTexture == null)
            _cellBackgroundTexture = new Texture2D(1, 1);
        _cellBackgroundTexture.SetPixel(0, 0, color);
        _cellBackgroundTexture.Apply();
        GUI.skin.box.normal.background = _cellBackgroundTexture;
        GUI.Box(position, GUIContent.none);
    }


    //-------------------------------------------------------------------------
    public float Property_Height_Elemet
    {
        get { return _listViewer.elementHeight; }
        set { _listViewer.elementHeight = value; }
    }
    public float Property_Height_Footer
    {
        get { return _listViewer.footerHeight; }
        set { _listViewer.footerHeight = value; }
    }
    public float Property_Height_Header
    {
        get { return _listViewer.headerHeight; }
        set { _listViewer.headerHeight = value; }
    }

    public bool Property_Show_AddButton
    {
        get { return _listViewer.displayAdd; }
        set { _listViewer.displayAdd = value; }
    }
    public bool Property_Show_RemoveButton
    {
        get { return showRemoveButtonNextElement; }
        set { showRemoveButtonNextElement = value; }
    }


    public bool Property_Show_Dragable
    {
        get { return _listViewer.draggable; }
        set { _listViewer.draggable = value; }
    }
    public bool Property_Show_DefaultBackground
    {
        get { return _listViewer.showDefaultBackground; }
        set { _listViewer.showDefaultBackground = value; }
    }
    public bool Property_Show_Header
    {
        get
        {
            return Math.Abs(_listViewer.headerHeight) > 0.01f;
        }
        set
        {
            if (value)
                _listViewer.headerHeight= Property_Height_Header;
            else _listViewer.headerHeight = 0;
        }
    }
    public bool Property_Show_HighlighSelected
    {
        get { return highlightSelected; }
        set { highlightSelected = value; }
    }


    public int Property_Count
    {
        get { return _list.Count; }
    }
    public IList<t> Property_List
    {
        get { return _list; }
    }
    //-------------------------------------------------------------------------

    public void DoList(Rect rect)
    {
        _listViewer.DoList(rect);
    }
    public void DoLayoutList()
    {
        _listViewer.DoLayoutList();
    }

}