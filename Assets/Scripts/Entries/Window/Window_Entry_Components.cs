using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rotorz.ReorderableList;
using UnityEditor;
using UnityEngine;

public class Window_Entry_Components : Window_EntryBase
{


    //data variables----------------------------------------------------------------
    public new Entry_Components EntryData
    {
        get { return base.EntryData as Entry_Components; }
    }

    //UI window variables-----------------------------------------------------------
    protected EntryComponentListAdaptor<EntryComponent> _componentsListadapter;
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


    //messages----------------------------------------------------------------------
    protected override void OnGUI()
    {

        if (EntryData == null )
        {

            EditorGUILayout.LabelField("Please initialize the window by calling the \"Initialize\" method in order to view the content.");
            return;
        }
        if(_componentsListControl == null || _componentsListadapter == null)
            Initialize(EntryData);
        
        DrawListHeader();
        UpdateListMode();
        DrawList();
        this.Repaint();
    }
    protected void DrawListHeader()
    {
        EditorGUILayout.BeginHorizontal();
        //draw component title
        ReorderableListGUI.Title("Components");

        //draw add button
        if(EntryData.ShowAddButton)
            if (GUILayout.Button("Add", EditorStyles.miniButtonLeft, GUILayout.Width(40), GUILayout.Height(20)))
                OnAddComponentClick(EntryData.Componets);

        //draw settings button
        if (GUILayout.Button("Settings", EditorStyles.miniButtonRight, GUILayout.Width(60), GUILayout.Height(20)))
            SettingsGenericMenu.ShowAsContext();
        
        EditorGUILayout.EndHorizontal();
    }
    protected void UpdateListMode()
    {
        if (EntryData.ShowEditMode)
            _componentsListControl.Flags = 0
                                           & ~ReorderableListFlags.DisableReordering
                                           & ~ReorderableListFlags.HideRemoveButtons
                                           & ~ReorderableListFlags.DisableContextMenu
                                           | ReorderableListFlags.DisableDuplicateCommand
                                           | ReorderableListFlags.DisableAutoFocus
                                           & ~ReorderableListFlags.ShowIndices
                                           //| ~ReorderableListFlags.DisableClipping
                                           & ~ReorderableListFlags.DisableAutoScroll;


        else
            _componentsListControl.Flags = 0
                                           | ReorderableListFlags.DisableReordering
                                           | ReorderableListFlags.HideRemoveButtons
                                           | ReorderableListFlags.DisableContextMenu
                                           | ReorderableListFlags.DisableDuplicateCommand
                                           | ReorderableListFlags.DisableAutoFocus
                                           & ~ReorderableListFlags.ShowIndices
                                           //| ~ReorderableListFlags.DisableClipping
                                           & ~ReorderableListFlags.DisableAutoScroll;


        if (EntryData.ShowAddButton)
            _componentsListControl.Flags &=  ~ReorderableListFlags.HideAddButton;
        else
            _componentsListControl.Flags |= ReorderableListFlags.HideAddButton;

    }
    protected void DrawList()
    {
        scrollerPos = EditorGUILayout.BeginScrollView(scrollerPos);
        _componentsListControl.Draw(_componentsListadapter);
        EditorGUILayout.EndScrollView();
    }


    protected virtual EntryComponent ComponentItemDrawer(Rect position, EntryComponent item)
    {
        item.ShowFieldTypeLabel = EntryData.ShowComponentsTypeLabel;
        item.IsInEditMode = EntryData.ShowEditMode;
        item.DrawView(ref position);
        this.Repaint();
        return item;
    }
    protected virtual void OnAddComponentClick(IList<EntryComponent> list)
    {
        //todo?: filter all components that are usable from the non usable
        if (_addComponentGenericMenu == null)
        {
            _addComponentGenericMenu = new GenericMenu();


            //base type
            Type abstractType = typeof(EntryComponent);

            //get all sub types
            var componentTypes = (from t in Assembly.GetExecutingAssembly().GetTypes()
                where t.IsClass && t.IsPublic && !t.IsAbstract && abstractType.IsAssignableFrom(t)
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

                    });
            }
        }

        _addComponentGenericMenu.ShowAsContext();

    }


    //custom methods----------------------------------------------------------------
    public override void Initialize(EntryBase data)
    {
        if(data == null)
            throw new ArgumentNullException();

        base.Initialize(data);
        _componentsListControl = new ReorderableListControl();
        _componentsListadapter = new EntryComponentListAdaptor<EntryComponent>(EntryData.Componets, ComponentItemDrawer, OnAddComponentClick);

    }

}