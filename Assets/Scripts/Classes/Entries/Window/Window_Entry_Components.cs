using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


public class Window_Entry_Components : Window_EntryBase
{


    //data variables----------------------------------------------------------------
    public new Entry_Components EntryData
    {
        get { return base.EntryData as Entry_Components; }
    }

    //UI window variables-----------------------------------------------------------
    private ExternalReorderableListAdapter<EntryComponent> _componentsListadapter;
    protected Vector2 scrollerPos;



    public GenericMenu SettingsGenericMenu
    {
        get
        {
            var _genericMenu = new GenericMenu();

            if (EntryData.ShowEditModeOption)
                _genericMenu.AddItem(new GUIContent("Show Edit Mode"), EntryData.ShowEditMode,
                    () =>
                    {
                        EntryData.ShowEditMode = !EntryData.ShowEditMode;
                    });


            _genericMenu.AddItem(new GUIContent("Show Component Types"), EntryData.ShowComponentsTypeLabel,
                () =>
                {
                    EntryData.ShowComponentsTypeLabel = !EntryData.ShowComponentsTypeLabel;
                });

            return _genericMenu;
        }
    }
    internal ExternalReorderableListAdapter<EntryComponent> ComponentsReorderableList
    {
        get
        {
            if(_componentsListadapter == null && EntryData!= null)
                InitializeComponentsReorderableList();
            return _componentsListadapter;
        }
    }


    //messages----------------------------------------------------------------------
    public override void OnGUI()
    {

        if (EntryData == null )
        {

            EditorGUILayout.LabelField("Please initialize the window by calling the \"Initialize\" method in order to view the content.");
            return;
        }
        if(ComponentsReorderableList == null)
            Initialize(EntryData);

        // var rect = this.position;
        DoHeader();
        DrawList();

    }

    private Rect headerRect;
    protected void DoHeader()
    {
        var height = EntryComponent.SingleLineHeight;
        var width = GUILayoutUtility.GetRect(minSize.x, maxSize.x, height, height).width;
        if (Math.Abs(width - 1) > 0.1f)
        {
            headerRect.width = width;
            headerRect.height = height;
            headerRect.position = Vector2.zero;
        }
        
        GUILayout.BeginArea(headerRect);
        ComponentsReorderableList.DoHeader(headerRect);
        GUILayout.EndArea();

    }

    protected void DrawList()
    {
        scrollerPos = EditorGUILayout.BeginScrollView(scrollerPos);
        ComponentsReorderableList.DoLayoutList();
        EditorGUILayout.EndScrollView();
    }



    //custom methods----------------------------------------------------------------
    void InitializeComponentsReorderableList()
    {
        _componentsListadapter = new ExternalReorderableListAdapter<EntryComponent>(EntryData.Componets);
        _componentsListadapter.CallBack_List_OnAddOptions += OnAdd;
       // _componentsListadapter.CallBack_List_OnInsert += OnInsert;
        _componentsListadapter.CallBack_List_OnRemove += OnRemove;
        _componentsListadapter.CallBack_List_OnDuplicate += OnDuplicate;


        _componentsListadapter.CallBack_Setting_OnSelect+= OnSelect;
        _componentsListadapter.CallBack_Setting_OnChanged+= OnReorder;


        _componentsListadapter.Callback_Draw_ElementHeight += OnGetItemHeight;
        _componentsListadapter.Callback_Draw_Element+= ItemDrawer;
        _componentsListadapter.Callback_Draw_Header+= DrawHeader;


        _componentsListadapter.CallBack_Setting_CanAdd += list => EntryData.ShowAddButton;
        _componentsListadapter.CallBack_Setting_CanRemove += (list,i) => EntryData.ShowRemoveButton;
        _componentsListadapter.CallBack_Setting_CanShowContextMenu += () => EntryData.ShowAddButton;
        _componentsListadapter. Property_Show_Dragable = EntryData.ShowDraggableButton;


        _componentsListadapter.Property_Show_Header = false;
    }



    internal void DrawHeader(Rect rect)
    {
        //set label width
        var tempPos = rect;
        tempPos.width -= 100;

        //draw component title
        EditorGUI.LabelField(tempPos, "Components");

        //set add button size
        tempPos.x += tempPos.width;
        tempPos.width = 40;

        //draw add button
        if (EntryData.ShowAddButton)
            if (GUI.Button(tempPos, "Add", EditorStyles.miniButtonLeft))
                ComponentsReorderableList.DoAdd();

        //set settings button size
        tempPos.x += tempPos.width;
        tempPos.width = 60;

        //draw settings button
        if (GUI.Button(tempPos,"Settings", EditorStyles.miniButtonRight))
            SettingsGenericMenu.ShowAsContext();

    }

    internal virtual void ItemDrawer(IList<EntryComponent> list, Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = list[index];
        element.ShowFieldTypeLabel = EntryData.ShowComponentsTypeLabel;
        element.IsInEditMode = EntryData.ShowEditMode;
        element.DrawView(ref rect);
        element.OnEditModeModified += Repaint;
        element.OnViewModeModified += Repaint;
    }

    internal virtual void OnAdd(IList<EntryComponent> list, BetterGenericMenu betterGenericMenu)
    {
        InitializeAddButtonGenerucMenu(betterGenericMenu, type =>
        {
            list.Add(CreateNewComponent(type));
            EntryData.OnComponentChanged(ListChangeType.Add,list.Count-1,-1);
        });
    }
    internal virtual void OnRemove(IList<EntryComponent> entryComponents, int i)
    {

        AssetsPath.DestroyAsset(entryComponents[i]);
        entryComponents.RemoveAt(i);
        EntryData.OnComponentChanged(ListChangeType.Remove,i,-1);

    }
    internal virtual void OnDuplicate(IList<EntryComponent> entryComponents, int i)
    {
        entryComponents.Insert(i+1, null);
        entryComponents[i+1] = entryComponents[i].Clone() as EntryComponent;
        entryComponents[i+1].FieldName = EntryData.GetNextAvailableName();
        entryComponents[i + 1].OnEditModeModified += () =>
        {
            EntryData.ValidateFieldName(entryComponents[i + 1]);
            OnDataChanged(entryComponents[i + 1]);
        };

        EntryData.OnComponentChanged(ListChangeType.Duplicate,i,-1);

    }
    internal virtual float OnGetItemHeight(IList<EntryComponent> entryComponents, int i)
    {
        return entryComponents[i].GetPropertyHeight();
    }

    bool _itemSelected;
    private int _reorderSelectedIndex = -1;
    internal void OnSelect(ReorderableList reorderableList)
    {
        _reorderSelectedIndex = reorderableList.index;
        _itemSelected = true;
    }
    internal virtual void OnReorder(ReorderableList list)
    {
        if (_itemSelected)
        {
            EntryData.OnComponentChanged(ListChangeType.Reorder, _reorderSelectedIndex, list.index);
            _itemSelected = false;
        }

    }


    internal virtual void OnDataChanged(EntryComponent data)
    {
        var index = EntryData.Componets.IndexOf(data);
        EntryData.OnComponentChanged(ListChangeType.DataChanged, index, -1);
    }


    internal void InitializeAddButtonGenerucMenu(BetterGenericMenu menu, Action<Type> onContextMenuItemSelected)
    {

        //base type
        Type abstractType = typeof(EntryComponent);

        //get all sub types
        var componentTypes = (from t in Assembly.GetExecutingAssembly().GetTypes()
            where t.IsClass && t.IsPublic && !t.IsAbstract && abstractType.IsAssignableFrom(t)
                  && t.GetCustomAttributes(true).Any(x => x.GetType() == typeof(SelectableComponentAttribute))
            select t).ToList();

        //set contexts menu options and OnItemSelect
        foreach (var typee in componentTypes)
        {
            var componentType = typee;
            menu.AddItem(componentType.Name.Split('_')[1], false,
                () => onContextMenuItemSelected(componentType));
        }
    }
    public EntryComponent CreateNewComponent(Type componentType)
    {
        if(!typeof(EntryComponent).IsAssignableFrom(componentType))
            throw  new ArgumentException("incompatible type");
        var instance = EntryComponent.CreateInstance(componentType);
        instance.Initialize(EntryData, EntryData.GetNextAvailableName());
        instance.OnEditModeModified += () =>
        {
            EntryData.ValidateFieldName(instance);
            OnDataChanged(instance);
        };
        //EntryData.OnComponentChanged(ListChangeType.Add);
        return instance;
    }
    //custom methods----------------------------------------------------------------

}
