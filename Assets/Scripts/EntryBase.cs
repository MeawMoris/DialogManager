using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using Rotorz.ReorderableList;

#endif
using UnityEngine;


[Serializable]
public class EntryBase: ScriptableObject
{
    
}



[Serializable]
public partial class Entry :EntryBase
{
    //fields---------------------------------------------------------------------------------------------
    [SerializeField] private List<EntryComponent> _componets = new List<EntryComponent>();

    //constructors---------------------------------------------------------------------------------------
    public Entry(IEnumerable<EntryComponent> componets = null)
    {
       if(componets != null)
            Componets.AddRange(componets);

    }
    
    //properties-----------------------------------------------------------------------------------------
    public List<EntryComponent> Componets
    {
        get
        {
            if(_componets == null)
                _componets = new List<EntryComponent>();
            return _componets;
        }
        private set { _componets = value; }
    }

    //editor methods-------------------------------------------------------------------------------------

    //---------------------------------------------------------------------------------------------------
}



#if UNITY_EDITOR
public partial class Entry
{

    public virtual EntryWindow GetNewWindow()
    {
        var window = new EntryWindow();
        window.Initialize(this);
        return window; // todo to implement
    }
    public virtual EntryWindow GetVisableWindow()
    {
        var window = (EntryWindow)EditorWindow.GetWindow(typeof(EntryWindow));
        window.Initialize(this);
        return window; // todo to implement
    }

}
#endif








#if UNITY_EDITOR

public class EntryWindow : EditorWindow
{
    //static methods----------------------------------------------------------------

    [MenuItem("Tools/Conversation Entry")]
    public static void ShowWindow()
    {
        var window = (EntryWindow)EditorWindow.GetWindow(typeof(EntryWindow));
        window.Show();
    }

    //data variables----------------------------------------------------------------
    private Entry _conversaton;
    public Entry Conversaton
    {
        get { return _conversaton; }
        set { _conversaton = value; }
    }

    //UI window variables-----------------------------------------------------------
    private EntryComponentListAdaptor<EntryComponent> _componentsListadapter;
    private ReorderableListControl _componentsListControl;

    //messages----------------------------------------------------------------------


    protected virtual void OnGUI()
    {
        if (Conversaton == null)
        {
            EditorGUILayout.LabelField("Please initialize the window by calling the \"Initialize\" method in order to view the content.");
            return;
        }

       if(_componentsListControl == null)
            _componentsListControl = new ReorderableListControl();

        if (_componentsListadapter == null)
            _componentsListadapter = new EntryComponentListAdaptor<EntryComponent>(_conversaton.Componets, OnAddComponentClick, ComponentItemDrawer );

        _componentsListControl.Draw(_componentsListadapter);

    }


    private EntryComponent ComponentItemDrawer(Rect position, EntryComponent item)
    {
        item.DrawView(position);
        this.Repaint();
        return item;
    }
    private void OnAddComponentClick(GenericMenu genericMenu, IList<EntryComponent> list)
    {
        //todo show needed types
        //todo set select type window pop up filters

        //base type
        Type abstractType = typeof(EntryComponent);

        //get all sub types
        var componentTypes = (from t in Assembly.GetExecutingAssembly().GetTypes()
            where t.IsClass && t.IsPublic && !t.IsAbstract && abstractType.IsAssignableFrom(t) select t).ToList();

        //set contexts menu options and OnItemSelect
        foreach (var componentType in componentTypes)      
            genericMenu.AddItem(new GUIContent(componentType.Name),false,()=> list.Add(EntryComponent.CreateInstance(componentType)) );
        
        
    }




    //custom methods----------------------------------------------------------------
    public virtual void Initialize(Entry conversation)
    {
        Conversaton = conversation;

    }



}

#endif
