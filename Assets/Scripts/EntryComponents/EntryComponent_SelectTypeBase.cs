using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[Serializable]
public abstract class EntryComponent_SelectTypeBase : EntryComponent
{
    /*
     * todo: layer field
     * todo: tag field
     * */



    //fields--------------------------------------------------------------------------------------
    private string filterString;
    private List<Type> availableTypes;
    private List<Type> filteredTypes;
    [SerializeField] private string selectedTypeName;


    //properties----------------------------------------------------------------------------------
    protected string FilterString
    {
        get
        {
            if (FilteredTypes == null)
                UpdateFilteredList();

            return filterString;
        }
        set
        {
            if (value != filterString)
            {
                filterString = value;
                UpdateFilteredList();
            }
            else  filterString = value;

        }
    }
    protected List<Type> FilteredTypes
    {
        get
        {
            if (filteredTypes == null)
            {
                FilteredTypes = new List<Type>();
                UpdateFilteredList();
            }

            return filteredTypes;
        }
        set
        {
            if (value == null)
            {
                filteredTypes.Clear();
                SelectedIndex = -1;
            }
            filteredTypes = value;
        }
    }
    protected List<Type> AvailableTypes
    {
        get
        {
            if (availableTypes == null || availableTypes.Count==0)
                availableTypes = GetAvailableTypes();
            return availableTypes;
        }
    }
    protected int SelectedIndex
    {
        get
        {
            //check if a type is selected
            var selectedType = AvailableTypes.FirstOrDefault(x => selectedTypeName.Equals(x.FullName));

            //if a type is selected
            if (selectedType != null)
            {
                //if the type is not in the filtered list
                var index = FilteredTypes.IndexOf(selectedType);
                if (index == -1)
                {
                    //add the selected type to the filtered list
                    filteredTypes.Add(selectedType);
                    index = filteredTypes.Count - 1;
                }
                return index;
            }
            return -1;

        }
        set
        {
            bool tempChange = SelectedIndex != value;

            if (value < FilteredTypes.Count && value >= 0)
            {
                selectedTypeName = FilteredTypes[value].FullName;
            }
            else selectedTypeName = "";

            if(tempChange && OnEditModeModified!= null)
                OnEditModeModified();
        }
    }
    public Type SelectedType
    {
        get { return AvailableTypes.FirstOrDefault(x => selectedTypeName.Equals(x.FullName)); }
    }

    public abstract object Value { get; set; }
    protected virtual bool ShowSearchField { get { return true; } }
    protected virtual string FieldTypeFieldString
    {
        get { return "Field Type"; }
    }

    public override string FieldTypeLabel
    {
        get
        {
            return string.Format("{0} {1}", base.FieldTypeLabel,
                (SelectedType == null?"":string.Format("- {0}", SelectedType.Name)) );
        }
    }

    //constructors--------------------------------------------------------------------------------
    protected override void Initialize(string componentName = "Object field name")
    {
        //----------------------------------
        base.Initialize(componentName);        
        //----------------------------------
        selectedTypeName = "";
        SelectedIndex = -1;
        //----------------------------------
    }
    public virtual List<Type> GetAvailableTypes()
    {
        return (from t in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            where (t.IsPublic || t.IsNestedPublic) && !(t.IsAbstract && t.IsSealed) && !t.IsGenericType
            select t).ToList();
    }


    //IClonable interface-------------------------------------------------------------------------

    public override void CloneTo(EntryComponent other)
    {
        base.CloneTo(other);
        var otherInst = other as EntryComponent_SelectTypeBase;
        if (otherInst == null)
            throw new ArgumentException("types are not the same");             
        otherInst.selectedTypeName = selectedTypeName;
         
    }

    //methods-------------------------------------------------------------------------------------
    void UpdateFilteredList()
    {
        FilteredTypes = AvailableTypes.Where(x => x.Name.ToLower().Contains((filterString??"").ToLower())).ToList();

    }

    //editor methods------------------------------------------------------------------------------


#if UNITY_EDITOR
    protected override void Draw(ref Rect pos)
    {
        
        pos.height = SingleLineHeight;

        var selectedType = AvailableTypes.FirstOrDefault(x => selectedTypeName.Equals(x.FullName));

        if (selectedType == null)
        {
            EditorGUI.LabelField(pos, "please select a type in edit mode");
            pos.y += pos.height;
        }

        else                 
            DrawObjectField(ref pos);
       


    }
    protected abstract void DrawObjectField(ref Rect pos);

    protected override void DrawEdit(ref Rect pos)
    {
        base.DrawEdit(ref pos);

        if (ShowSearchField)
        {
            //draw filter sting field
            pos.height = SingleLineHeight;
            FilterString = EditorGUI.TextField(pos, "Filter Type", FilterString);
            pos.y += SingleLineHeight;

        }

        //draw the pop-up field

        lock (name)
        {
            SelectedIndex = EditorGUI.Popup(pos, FieldTypeFieldString, SelectedIndex, FilteredTypes.Select(x => x.Name).ToArray());
        }
        pos.y += SingleLineHeight;


    }

    public override float GetPropertyHeight()
    {
        float val = base.GetPropertyHeight();

        if (IsInEditMode)
            val += SingleLineHeight * ((ShowSearchField)?2:1);

        else if (SelectedType == null)
            val += SingleLineHeight;


        return val;
    }
#endif



}