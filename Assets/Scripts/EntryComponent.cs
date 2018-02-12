
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rotorz.ReorderableList;


[Serializable]
public class EntryComponent:UnityEngine.ScriptableObject,ICloneable
{
    //statics-------------------------------------------------------------------------------------
    public new static EntryComponent CreateInstance<t>() where t : EntryComponent
    {
        var value = ScriptableObject.CreateInstance<t>();
        value.Initialize();
        return value;
    }
    public new static EntryComponent CreateInstance(Type t)
    {
        var value = (EntryComponent)ScriptableObject.CreateInstance(t);
        value.Initialize();
        return value;
    }
    public static float SingleLineHeight
    {
        get { return ReorderableListGUI.DefaultItemHeight; }
    }

    //fields--------------------------------------------------------------------------------------
    [SerializeField] private string _fieldName;
    [SerializeField] private bool _isInEditMode;


    //properties----------------------------------------------------------------------------------
    public string FieldName
    {
        get { return _fieldName; }
        set { _fieldName = value; }
    }
    public bool IsInEditMode
    {
        get { return _isInEditMode; }
        set { _isInEditMode = value; }
    }
  

    //constructors--------------------------------------------------------------------------------
    public virtual void Initialize(string componentName = "Field Name")
    {
        FieldName = componentName;
        _isInEditMode = false;
    }


    //IClonable interface-------------------------------------------------------------------------
    public virtual object Clone()
    {
        var value = (EntryComponent) ScriptableObject.CreateInstance(this.GetType());
        value.Initialize(FieldName);
        return value;
    }


    //--------------------------------------------------------------------------------------------


    //editor methods------------------------------------------------------------------------------
#if UNITY_EDITOR

    public virtual void Draw(Rect pos)
    {
        pos.height = SingleLineHeight;
        if (IsInEditMode)
            FieldName = EditorGUI.TextField(pos,"Field Name" ,FieldName);

    }
    public virtual float GetPropertyHeight()
    {
        return IsInEditMode ? SingleLineHeight : 0;
    }
#endif
    //--------------------------------------------------------------------------------------------
}

public class EntryComponent_TextAreaField : EntryComponent
{
    //statics-------------------------------------------------------------------------------------
    private static float TextAreaHeight {get { return SingleLineHeight * 5; } }

    //fields--------------------------------------------------------------------------------------
    [SerializeField]private string textArea;

    //properties----------------------------------------------------------------------------------
    public string TextArea
    {
        get { return textArea; }
        set { textArea = value; }
    }

    //constructors--------------------------------------------------------------------------------
    public override void Initialize(string componentName = "text area field name")
    {
        base.Initialize(componentName);
        TextArea = "##text##";
    }
    

    //IClonable interface-------------------------------------------------------------------------
    public override object Clone()
    {
        var item = base.Clone();
        ((EntryComponent_TextAreaField) item).TextArea = TextArea;
        return item;

    }

    //editor methods------------------------------------------------------------------------------
#if UNITY_EDITOR
    public override void Draw(Rect pos)
    {
        base.Draw(pos);

        pos.y += base.GetPropertyHeight();

        if (!IsInEditMode)
        {
            pos.height = SingleLineHeight;
            EditorGUI.LabelField(pos, FieldName);
            pos.y += SingleLineHeight;
        }

        pos.height = TextAreaHeight;
        TextArea = EditorGUI.TextArea(pos, TextArea);

    }

    public override float GetPropertyHeight()
    {
        return TextAreaHeight + (IsInEditMode ? base.GetPropertyHeight() : SingleLineHeight);
    }
#endif

}

public class EntryComponent_ClassField : EntryComponent
{
    //statics-------------------------------------------------------------------------------------


    //fields--------------------------------------------------------------------------------------
    private string filterString;
    private Type fieldType;
    private int selectedIndex;
    private List<Type> availableTypes;
    private List<Type> filteredTypes;
    
    //properties----------------------------------------------------------------------------------
    public string FilterString
    {
        get { return filterString; }
        set
        {
            if (value != filterString)
            {
                filteredTypes = availableTypes.Where(x => x.Name.ToLower().Contains(value)).ToList();
               

                if (filteredTypes.Count ==0)
                    selectedIndex = -1;
                else
                    selectedIndex = 0;

                switch (value)
                {
                    case "int":
                        selectedIndex = filteredTypes.IndexOf(typeof(int));
                        if (selectedIndex == -1)
                        {
                            filteredTypes.Add(typeof(int));
                            selectedIndex = filteredTypes.Count - 1;
                        }
                        break;
                    case "string":
                        selectedIndex = filteredTypes.IndexOf(typeof(string));
                        if (selectedIndex == -1)
                        {
                            filteredTypes.Add(typeof(string));
                            selectedIndex = filteredTypes.Count - 1;
                        }
                        break;
                    case "float":
                        selectedIndex = filteredTypes.IndexOf(typeof(float));
                        if (selectedIndex == -1)
                        {
                            filteredTypes.Add(typeof(float));
                            selectedIndex = filteredTypes.Count - 1;
                        }
                        break;
                    case "double":
                        selectedIndex = filteredTypes.IndexOf(typeof(double));
                        if (selectedIndex == -1)
                        {
                            filteredTypes.Add(typeof(double));
                            selectedIndex = filteredTypes.Count - 1;
                        }
                        break;
                    case "monobehaviour":
                        selectedIndex = filteredTypes.IndexOf(typeof(MonoBehaviour));
                        if (selectedIndex == -1)
                        {
                            filteredTypes.Add(typeof(MonoBehaviour));
                            selectedIndex = filteredTypes.Count - 1;
                        }
                        break;
                    case "char":
                        selectedIndex = filteredTypes.IndexOf(typeof(char));
                        if (selectedIndex == -1)
                        {
                            filteredTypes.Add(typeof(char));
                            selectedIndex = filteredTypes.Count - 1;
                        }
                        break;
                    case "long":
                        selectedIndex = filteredTypes.IndexOf(typeof(long));
                        if (selectedIndex == -1)
                        {
                            filteredTypes.Add(typeof(long));
                            selectedIndex = filteredTypes.Count - 1;
                        }
                        break;
                }

            }
            filterString = value;
        }
    }

    //constructors--------------------------------------------------------------------------------
    public override void Initialize(string componentName = "class field name")
    {
        //----------------------------------
        base.Initialize(componentName);
        
        //----------------------------------
        if (filteredTypes == null)
            filteredTypes = new List<Type>();
        else filteredTypes.Clear();
        //----------------------------------
        availableTypes = (from t in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x=>x.GetTypes())
                          where (t.IsPublic || t.IsNestedPublic) && !(t.IsAbstract && t.IsSealed )
                          select t).ToList();
        //----------------------------------
        FilterString = "string";
        selectedIndex = 0;
        //----------------------------------
    }


    //IClonable interface-------------------------------------------------------------------------


    //editor methods------------------------------------------------------------------------------
#if UNITY_EDITOR
    public override void Draw(Rect pos)
    {
        base.Draw(pos);

        pos.y += base.GetPropertyHeight();

        if (IsInEditMode)
        {
            pos.height = SingleLineHeight;
            FilterString=  EditorGUI.TextField(pos,"Filter Type" ,FilterString);

            pos.y += SingleLineHeight;

            if (selectedIndex > filteredTypes.Count)
                selectedIndex = -1;

            selectedIndex = EditorGUI.Popup(pos, selectedIndex, filteredTypes.Select(x=>x.Name).ToArray());

        }


    }

    public override float GetPropertyHeight()
    {
        return base.GetPropertyHeight() + SingleLineHeight * (IsInEditMode ? 2 : 1);
    }
#endif

}

