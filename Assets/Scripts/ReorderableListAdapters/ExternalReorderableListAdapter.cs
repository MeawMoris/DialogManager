using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class ExternalReorderableListAdapter<t>
{
    public delegate void ElementDrawCallbackDelegate(IList<t> list ,Rect rect, int index, bool isActive, bool isFocused);
    protected const float SingleLineHeight = 20;

    //--------------------------------------------------------------------------
    private ReorderableList _listViewer;
    private IList<t> _list;
    private int _reorderInitialIndex = 0;
    private bool showRemoveButtonNextElement ;
    private bool _showContextMenu ;
    //events--------------------------------------------------------------------
    public Action<IList<t>> _callBack_List_OnAdd;
    public Action<IList<t>, BetterGenericMenu> _callBack_List_OnAddOptions;
    public Action<IList<t>> CallBack_List_OnAdd
    {
        get { return _callBack_List_OnAdd; }
        set
        {
            _callBack_List_OnAddOptions = null;
            _addOptions = null;
            _callBack_List_OnAdd = value;
        }
    }
    public Action<IList<t>, BetterGenericMenu> CallBack_List_OnAddOptions
    {
        get { return _callBack_List_OnAddOptions; }
        set
        {
            _callBack_List_OnAdd = null;
            _callBack_List_OnAddOptions = value;
        }
    }

    private BetterGenericMenu _addOptions;
    public BetterGenericMenu AddOptions
    {
        get
        {
            if(_addOptions == null)
                _addOptions = new BetterGenericMenu();
            return _addOptions;
        }
    }

    //events--------------------------------------------------------------------

    public ReorderableList.AddDropdownCallbackDelegate CallBack_List_onAddDropdownCallback;

    public Action<IList<t>, int> CallBack_List_OnRemove;
    // public Action<IList<t>, int> CallBack_List_OnInsert;
    public Action<IList<t>, int> CallBack_List_OnDuplicate;
    //public Action<IList<t>, int, int> CallBack_List_OnReorder;//on move


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

    public Func<IList<t>,int,bool> CallBack_Setting_CanRemove;
    public Func<bool> CallBack_Setting_CanShowContextMenu;

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
        _showContextMenu = true;
        _listViewer.displayRemove = false;
        Property_Height_Header = SingleLineHeight;
        Property_Height_Elemet = SingleLineHeight;

        //add index
        _listViewer.onAddCallback += (x) => Add();

        //remove index
        _listViewer.onRemoveCallback += reorderableList => Remove(reorderableList.index);

        //draw events
        _listViewer.drawElementCallback += DrawElementCallback;
        _listViewer.drawElementBackgroundCallback += DrawElementBackgroundCallback;
        _listViewer.elementHeightCallback += GetItemHeight;

    }


    protected void Add()
    {
        if (_callBack_List_OnAdd != null)
            _callBack_List_OnAdd(_list);
        else if (_callBack_List_OnAddOptions != null)
        {
            AddOptions.Clear();
            _callBack_List_OnAddOptions(_list, AddOptions);
            AddOptions.ShowAsContext();

        }
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
    protected void InsertWithAdd(int index)//todo to implement here
    {
        Add();
        _list.Insert(index, _list[_list.Count - 1]);
        _list.RemoveAt(_list.Count - 1);
    }


    //-------------------------------------------------------------------------


    private void DrawElementBackgroundCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        if (index >= _list.Count)
            return;

        //show context menu
        if (_showContextMenu && (CallBack_Setting_CanShowContextMenu== null || CallBack_Setting_CanShowContextMenu()))
        {
            var elementDrawPos = (rect.position);
            var ElementSize = (rect.size);
            var mousePos = Event.current.mousePosition;
            if (Event.current.button == 1 && Property_Show_AddButton && CallBack_Setting_CanAdd(_listViewer))

                if (mousePos.x > elementDrawPos.x && mousePos.x < elementDrawPos.x + ElementSize.x
                    && mousePos.y > elementDrawPos.y && mousePos.y < elementDrawPos.y + ElementSize.y)
                {

                    GenericMenu options = new GenericMenu();

                    if (_callBack_List_OnAdd != null)
                    {
                        options.AddItem(new GUIContent("Insert Above"), false, () => InsertWithAdd(index));
                        options.AddItem(new GUIContent("Insert Below"), false, () => InsertWithAdd(index + 1));
                    }
                    if (CallBack_List_OnDuplicate != null)
                    {
                        if (_callBack_List_OnAdd != null || _callBack_List_OnAddOptions != null)
                            options.AddSeparator("");
                        options.AddItem(new GUIContent("Duplicate"), false, () => Duplicate(index));
                    }


                    _listViewer.ReleaseKeyboardFocus();
                    //_listViewer.GrabKeyboardFocus();


                    options.ShowAsContext();
                    //  isFocused = true;
                }

            if (Callback_Draw_ElementBackground != null)
                Callback_Draw_ElementBackground(_list, rect, index, isActive, isFocused);
        }

        //draw selected background
        if (isFocused && _showContextMenu)
            DrawQuad(rect, 0, 50, 186, .3f);
    }
    private void DrawElementCallback(Rect pos, int index, bool isActive, bool isFocused)
    {
        if (index>=_list.Count)
            return;

        //if can remove, set the field size 
        var rect = pos ;
        var canRemove = (CallBack_Setting_CanRemove==null) || CallBack_Setting_CanRemove(_list,index);

        if (CallBack_List_OnRemove != null && showRemoveButtonNextElement && canRemove)
            rect.width -= SingleLineHeight;

        //draw the element
        if (Callback_Draw_Element != null)
            Callback_Draw_Element(_list, rect, index, isActive, isFocused);

        //if can remove, show remove button 
        if (CallBack_List_OnRemove != null &&showRemoveButtonNextElement && canRemove)
        {
            //calculate remove button pos
            rect.height = rect.width = SingleLineHeight;
            rect.x = pos.x + pos.width - SingleLineHeight;
            rect.y = pos.y + GetItemHeight(index) / 2 - rect.height / 2;

            //on remove button pressed

            if (GUI.Button(rect, ReorderableList.defaultBehaviours.iconToolbarMinus,
                ReorderableList.defaultBehaviours.preButton))
            {
                _listViewer.ReleaseKeyboardFocus();
                Remove(index);
            }
        }


    }
    protected float GetItemHeight(int index)
    {
        if (index >= _list.Count)
            return _listViewer.elementHeight;

        if (Callback_Draw_ElementHeight != null)
            return Callback_Draw_ElementHeight(_list, index);

        return _listViewer.elementHeight;
    }


    private Texture2D _cellBackgroundTexture;
    protected void DrawQuad(Rect position, int r, int g, int b, float a = 1)
    {
        Color color = new Color(r / 255f, g / 255f, b / 255f, a);
        if(_cellBackgroundTexture == null)
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
        get { return _showContextMenu; }
        set { _showContextMenu = value; }
    }
    public bool Property_Show_ContextMenu
    {
        get { return _showContextMenu; }
        set { _showContextMenu = value; }
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

    public void DoAdd()
    {
        Add();
    }
    //-------------------------------------------------------------------------

}