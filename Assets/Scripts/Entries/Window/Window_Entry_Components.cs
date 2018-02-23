using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rotorz.ReorderableList;
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
    protected ReorderableListControl _componentsListControl;
    protected GenericMenu _addComponentGenericMenu;
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
    protected ExternalReorderableListAdapter<EntryComponent> ComponentsListadapter
    {
        get
        {
            if(_componentsListadapter == null)
                InitializeComponentsReorderableList();
            return _componentsListadapter;
        }
    }
    protected ReorderableListControl ComponentsListControl
    {
        get
        {
            if(_componentsListControl== null)
                _componentsListControl = new ReorderableListControl();

            return _componentsListControl;
        }
    }

    //messages----------------------------------------------------------------------
    protected override void OnGUI()
    {

        if (EntryData == null )
        {

            EditorGUILayout.LabelField("Please initialize the window by calling the \"Initialize\" method in order to view the content.");
            return;
        }
        if(ComponentsListControl == null || ComponentsListadapter == null)
            Initialize(EntryData);
        
       // DrawListHeader();
        UpdateListMode();
        DrawList();
        this.Repaint();
    }

    protected void UpdateListMode()
    {
        if (EntryData.ShowEditMode)
            ComponentsListControl.Flags = 0
                                           & ~ReorderableListFlags.DisableContextMenu
                                           | ReorderableListFlags.DisableDuplicateCommand
                                           | ReorderableListFlags.DisableAutoFocus
                                           & ~ReorderableListFlags.ShowIndices
                                           //| ~ReorderableListFlags.DisableClipping
                                           & ~ReorderableListFlags.DisableAutoScroll;


        else
            ComponentsListControl.Flags = 0
                                           | ReorderableListFlags.DisableContextMenu
                                           | ReorderableListFlags.DisableDuplicateCommand
                                           | ReorderableListFlags.DisableAutoFocus
                                           & ~ReorderableListFlags.ShowIndices
                                           //| ~ReorderableListFlags.DisableClipping
                                           & ~ReorderableListFlags.DisableAutoScroll;


        if (EntryData.ShowAddButton)
            ComponentsListControl.Flags &=  ~ReorderableListFlags.HideAddButton;
        else
            ComponentsListControl.Flags |= ReorderableListFlags.HideAddButton;

        if (EntryData.ShowRemoveButton)
            ComponentsListControl.Flags &= ~ReorderableListFlags.HideRemoveButtons;
        else
            ComponentsListControl.Flags |= ReorderableListFlags.HideRemoveButtons;

        if (EntryData.ShowDraggableButton)
            ComponentsListControl.Flags &= ~ReorderableListFlags.DisableReordering;
        else
            ComponentsListControl.Flags |= ReorderableListFlags.DisableReordering;


    }

    protected void DrawListHeader()
    {
        EditorGUILayout.BeginHorizontal();
        //draw component title
        ReorderableListGUI.Title("Components");

        //draw add button
        if(EntryData.ShowAddButton)
            if (GUILayout.Button("Add", EditorStyles.miniButtonLeft, GUILayout.Width(40), GUILayout.Height(20)))
                OnAdd(EntryData.Componets);

        //draw settings button
        if (GUILayout.Button("Settings", EditorStyles.miniButtonRight, GUILayout.Width(60), GUILayout.Height(20)))
            SettingsGenericMenu.ShowAsContext();
        
        EditorGUILayout.EndHorizontal();
    }
    protected void DrawList()
    {
        scrollerPos = EditorGUILayout.BeginScrollView(scrollerPos);
        ComponentsListadapter.DoLayoutList();
        EditorGUILayout.EndScrollView();
    }



    //custom methods----------------------------------------------------------------
    void InitializeComponentsReorderableList()
    {
        _componentsListadapter = new ExternalReorderableListAdapter<EntryComponent>(EntryData.Componets);
        _componentsListadapter.CallBack_List_OnAdd += OnAdd;
        _componentsListadapter.CallBack_List_OnInsert += OnInsert;
        _componentsListadapter.CallBack_List_OnRemove += OnRemove;
        _componentsListadapter.CallBack_List_OnDuplicate += OnDuplicate;
        _componentsListadapter.Callback_Draw_ElementHeight += OnGetItemHeight;
        _componentsListadapter.CallBack_List_OnReorder+= OnMove;
        _componentsListadapter.Callback_Draw_Element+= ItemDrawer;
        _componentsListadapter.Callback_Draw_Header+= DrawHeader;
        _componentsListadapter.CallBack_Setting_CanAdd += list => EntryData.ShowAddButton;
        _componentsListadapter.CallBack_Setting_CanRemove += list => EntryData.ShowRemoveButton;
        //_componentsListadapter.Property_Show_Header = false;

    }


    protected void DrawHeader(Rect rect)
    {
        //set label width
        var tempPos = rect;
        tempPos.width -= 100;

        //draw component title
        EditorGUI.LabelField(tempPos, "Components");

        //set add button size
        tempPos.x += tempPos.width;
        tempPos.height = 20;
        tempPos.width = 40;

        //draw add button
        if (EntryData.ShowAddButton)
            if (GUI.Button(tempPos, "Add", EditorStyles.miniButtonLeft))
                OnAdd(EntryData.Componets);

        //set settings button size
        tempPos.x += tempPos.width;
        tempPos.width = 60;

        //draw settings button
        if (GUI.Button(tempPos,"Settings", EditorStyles.miniButtonRight))
            SettingsGenericMenu.ShowAsContext();

    }

    protected virtual void ItemDrawer(IList<EntryComponent> list, Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = list[index];
        element.ShowFieldTypeLabel = EntryData.ShowComponentsTypeLabel;
        element.IsInEditMode = EntryData.ShowEditMode;
        element.DrawView(ref rect);
        element.OnEditModeModified += Repaint;
        element.OnViewModeModified += Repaint;
    }

    protected virtual void OnAdd(IList<EntryComponent> list)
    {
        if (_addComponentGenericMenu == null)
        {
            _addComponentGenericMenu = new GenericMenu();


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
                _addComponentGenericMenu.AddItem(new GUIContent(componentType.Name.Split('_')[1]), false,
                    () =>
                    {
                        var instance = EntryComponent.CreateInstance(componentType);
                        instance.Initialize(EntryData, EntryData.GetNextAvailableName());
                        instance.OnEditModeModified += () => { EntryData.ValidateFieldName(instance); };
                        list.Add(instance);
                        EntryData.OnComponentChanged(ListChangeType.Add);
                    });
            }
        }

        _addComponentGenericMenu.ShowAsContext();
    }
    protected virtual void OnInsert(IList<EntryComponent> entryComponents, int i)
    {
        OnAdd(entryComponents);
        entryComponents.Insert(i, entryComponents[entryComponents.Count - 1]);
        entryComponents.RemoveAt(entryComponents.Count - 1);
    }
    protected virtual void OnRemove(IList<EntryComponent> entryComponents, int i)
    {
        entryComponents.RemoveAt(i);
        EntryData.OnComponentChanged(ListChangeType.Remove);

    }
    protected virtual void OnDuplicate(IList<EntryComponent> entryComponents, int i)
    {
        entryComponents.Insert(i, null);
        entryComponents[i] = entryComponents[i + 1].Clone() as EntryComponent;
        EntryData.OnComponentChanged(ListChangeType.Duplicate);

    }
    protected virtual float OnGetItemHeight(IList<EntryComponent> entryComponents, int i)
    {
        return entryComponents[i].GetPropertyHeight();
    }
    protected virtual void OnMove(IList<EntryComponent> entryComponents, int oldPos, int newPos)
    {

        var temp = entryComponents[newPos];
        entryComponents[newPos] = entryComponents[oldPos];
        entryComponents[oldPos] = temp;
    }


    //custom methods----------------------------------------------------------------


}

public class Window_Entry_Components_Copy : Window_EntryBase
{


    //data variables----------------------------------------------------------------
    public new Entry_Components EntryData
    {
        get { return base.EntryData as Entry_Components; }
    }

    //UI window variables-----------------------------------------------------------
    private ExternalListAdapter<EntryComponent> _componentsListadapter;
    protected ReorderableListControl _componentsListControl;
    protected GenericMenu _addComponentGenericMenu;
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
    protected ExternalListAdapter<EntryComponent> ComponentsListadapter
    {
        get
        {
            if (_componentsListadapter == null)
                InitializeComponentsReorderableList();
            return _componentsListadapter;
        }
    }
    protected ReorderableListControl ComponentsListControl
    {
        get
        {
            if (_componentsListControl == null)
                _componentsListControl = new ReorderableListControl();

            return _componentsListControl;
        }
    }

    //messages----------------------------------------------------------------------
    protected override void OnGUI()
    {

        if (EntryData == null)
        {

            EditorGUILayout.LabelField("Please initialize the window by calling the \"Initialize\" method in order to view the content.");
            return;
        }
        if (ComponentsListControl == null || ComponentsListadapter == null)
            Initialize(EntryData);

        DrawListHeader();
        UpdateListMode();
        DrawList();
        this.Repaint();
    }

    protected void UpdateListMode()
    {
        if (EntryData.ShowEditMode)
            ComponentsListControl.Flags = 0
                                           & ~ReorderableListFlags.DisableContextMenu
                                           | ReorderableListFlags.DisableDuplicateCommand
                                           | ReorderableListFlags.DisableAutoFocus
                                           & ~ReorderableListFlags.ShowIndices
                                           //| ~ReorderableListFlags.DisableClipping
                                           & ~ReorderableListFlags.DisableAutoScroll;


        else
            ComponentsListControl.Flags = 0
                                           | ReorderableListFlags.DisableContextMenu
                                           | ReorderableListFlags.DisableDuplicateCommand
                                           | ReorderableListFlags.DisableAutoFocus
                                           & ~ReorderableListFlags.ShowIndices
                                           //| ~ReorderableListFlags.DisableClipping
                                           & ~ReorderableListFlags.DisableAutoScroll;


        if (EntryData.ShowAddButton)
            ComponentsListControl.Flags &= ~ReorderableListFlags.HideAddButton;
        else
            ComponentsListControl.Flags |= ReorderableListFlags.HideAddButton;

        if (EntryData.ShowRemoveButton)
            ComponentsListControl.Flags &= ~ReorderableListFlags.HideRemoveButtons;
        else
            ComponentsListControl.Flags |= ReorderableListFlags.HideRemoveButtons;

        if (EntryData.ShowDraggableButton)
            ComponentsListControl.Flags &= ~ReorderableListFlags.DisableReordering;
        else
            ComponentsListControl.Flags |= ReorderableListFlags.DisableReordering;


    }

    protected void DrawListHeader()
    {
        EditorGUILayout.BeginHorizontal();
        //draw component title
        ReorderableListGUI.Title("Components");

        //draw add button
        if (EntryData.ShowAddButton)
            if (GUILayout.Button("Add", EditorStyles.miniButtonLeft, GUILayout.Width(40), GUILayout.Height(20)))
                OnAdd(EntryData.Componets);

        //draw settings button
        if (GUILayout.Button("Settings", EditorStyles.miniButtonRight, GUILayout.Width(60), GUILayout.Height(20)))
            SettingsGenericMenu.ShowAsContext();

        EditorGUILayout.EndHorizontal();
    }
    protected void DrawList()
    {
        scrollerPos = EditorGUILayout.BeginScrollView(scrollerPos);
        ComponentsListControl.Draw(ComponentsListadapter);
        EditorGUILayout.EndScrollView();
    }



    //custom methods----------------------------------------------------------------
    void InitializeComponentsReorderableList()
    {
        _componentsListadapter = new ExternalListAdapter<EntryComponent>(EntryData.Componets, ItemDrawer);
        ComponentsListadapter.OnAdd += OnAdd;
        ComponentsListadapter.OnRemove += OnRemove;
        ComponentsListadapter.OnDuplicate += OnDuplicate;
        ComponentsListadapter.OnInsert += OnInsert;
        ComponentsListadapter.OnGetItemHeight += OnGetItemHeight;
        ComponentsListadapter.OnMove += OnMove;

    }


    protected virtual EntryComponent ItemDrawer(Rect position, EntryComponent item)
    {
        item.ShowFieldTypeLabel = EntryData.ShowComponentsTypeLabel;
        item.IsInEditMode = EntryData.ShowEditMode;
        item.DrawView(ref position);
        item.OnEditModeModified += Repaint;
        item.OnViewModeModified += Repaint;

        return item;
    }


    protected virtual void OnAdd(IList<EntryComponent> list)
    {
        if (_addComponentGenericMenu == null)
        {
            _addComponentGenericMenu = new GenericMenu();


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
                _addComponentGenericMenu.AddItem(new GUIContent(componentType.Name.Split('_')[1]), false,
                    () =>
                    {
                        var instance = EntryComponent.CreateInstance(componentType);
                        instance.Initialize(EntryData, EntryData.GetNextAvailableName());
                        instance.OnEditModeModified += () => { EntryData.ValidateFieldName(instance); };
                        list.Add(instance);
                        EntryData.OnComponentChanged(ListChangeType.Add);
                    });
            }
        }

        _addComponentGenericMenu.ShowAsContext();
    }
    protected virtual void OnInsert(IList<EntryComponent> entryComponents, int i)
    {
        OnAdd(entryComponents);
        entryComponents.Insert(i, entryComponents[entryComponents.Count - 1]);
        entryComponents.RemoveAt(entryComponents.Count - 1);
    }
    protected virtual void OnRemove(IList<EntryComponent> entryComponents, int i)
    {
        entryComponents.RemoveAt(i);
        EntryData.OnComponentChanged(ListChangeType.Remove);

    }
    protected virtual void OnDuplicate(IList<EntryComponent> entryComponents, int i)
    {
        entryComponents.Insert(i, null);
        entryComponents[i] = entryComponents[i + 1].Clone() as EntryComponent;
        EntryData.OnComponentChanged(ListChangeType.Duplicate);

    }
    protected virtual float OnGetItemHeight(IList<EntryComponent> entryComponents, int i)
    {
        return entryComponents[i].GetPropertyHeight();
    }
    protected virtual void OnMove(IList<EntryComponent> entryComponents, int oldPos, int newPos)
    {
        if (newPos > oldPos)
            newPos--;
        var temp = entryComponents[newPos];
        entryComponents[newPos] = entryComponents[oldPos];
        entryComponents[oldPos] = temp;
    }


    //custom methods----------------------------------------------------------------


}