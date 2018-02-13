
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
public abstract class EntryComponent:UnityEngine.ScriptableObject,ICloneable
{
    //statics-------------------------------------------------------------------------------------
    public new static EntryComponent CreateInstance<t>() where t : EntryComponent
    {
        return CreateInstance(typeof(t));
    }
    public new static EntryComponent CreateInstance(Type t)
    {
        if (t.IsAbstract || t.IsNotPublic)
            return null;

        var value = (EntryComponent)ScriptableObject.CreateInstance(t);
        value.Initialize();
        return value;
    }


    //fields--------------------------------------------------------------------------------------
    [SerializeField] private string _fieldName;
    [SerializeField] private bool _isInEditMode;
    [SerializeField] private bool _isInitialized;

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
        _isInitialized = true;
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
    public static float SingleLineHeight
    {
        get { return ReorderableListGUI.DefaultItemHeight; }
    }

    public void DrawView(Rect pos)
    {
        if (IsInEditMode)
            DrawEdit(ref pos);
        else Draw(ref pos);
    }
    protected virtual void Draw(ref Rect pos)
    {

    }
    protected virtual void DrawEdit(ref Rect pos)
    {
        if (!_isInitialized)
            Initialize(FieldName);

        pos.height = SingleLineHeight;
        if (IsInEditMode)
            FieldName = EditorGUI.TextField(pos, "Field Name", FieldName);

    }

    public virtual float GetPropertyHeight()
    {
        return IsInEditMode ? SingleLineHeight : 0;
    }
#endif
    //--------------------------------------------------------------------------------------------
}

[Serializable]
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
    protected override void Draw(ref Rect pos)
    {
        base.Draw(ref pos);
        pos.y += base.GetPropertyHeight();


        pos.height = SingleLineHeight;
        EditorGUI.LabelField(pos, FieldName);
        pos.y += SingleLineHeight;

        pos.height = TextAreaHeight;
        TextArea = EditorGUI.TextArea(pos, TextArea);

    }

    protected override void DrawEdit(ref Rect pos)
    {
        base.DrawEdit(ref pos);
        pos.y += base.GetPropertyHeight();

        pos.height = TextAreaHeight;
        TextArea = EditorGUI.TextArea(pos, TextArea);

    }

    public override float GetPropertyHeight()
    {
        return TextAreaHeight + (IsInEditMode ? base.GetPropertyHeight() : SingleLineHeight);
    }
#endif

}

[Serializable]
public abstract class EntryComponent_ObjectField : EntryComponent
{
    //todo serialize object field
    //todo check why getWindow height is off a couple of seconds
    //fields--------------------------------------------------------------------------------------
     private string filterString;
     private List<Type> availableTypes;
     private List<Type> filteredTypes;
    [SerializeField] private string selectedTypeName;
    [SerializeField] private object data;

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

            var selectedType = AvailableTypes.FirstOrDefault(x => selectedTypeName.Equals(x.FullName));
            if (selectedType != null)
            {
                var index = filteredTypes.IndexOf(selectedType);
                if (index == -1)
                {
                    filteredTypes.Add(selectedType);
                    index = filteredTypes.Count - 1;
                }
                return index;
            }
            return -1;

        }
        set
        {
            if (value < FilteredTypes.Count && value >= 0)
            {
                selectedTypeName = FilteredTypes[value].FullName;
            }
            else selectedTypeName = "";

        }
    }
    public Type SelectedType
    {
        get { return AvailableTypes.FirstOrDefault(x => selectedTypeName.Equals(x.FullName)); }
    }
    public object Value { get { return data; } protected set { data = value; }}



    //constructors--------------------------------------------------------------------------------
    public override void Initialize(string componentName = "Object field name")
    {
        //----------------------------------
        base.Initialize(componentName);        
        //----------------------------------
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
    public override object Clone()
    {
        var item = base.Clone() as EntryComponent_UnityObjectField;
        item.selectedTypeName = selectedTypeName;
        return item;
    }

    //methods-------------------------------------------------------------------------------------
    void UpdateFilteredList()
    {
        FilteredTypes = AvailableTypes.Where(x => x.Name.ToLower().Contains(filterString??"")).ToList();

    }

    //editor methods------------------------------------------------------------------------------


#if UNITY_EDITOR
    protected override void Draw(ref Rect pos)
    {
        base.Draw(ref pos);
        pos.y += base.GetPropertyHeight();
        pos.height = SingleLineHeight;



        var selectedType = AvailableTypes.FirstOrDefault(x => selectedTypeName.Equals(x.FullName));

        if (selectedType == null)
            EditorGUI.LabelField(pos, "please select a type in edit mode");

        else                 
            DrawObjectField(ref pos);
       


    }
    protected abstract void DrawObjectField(ref Rect pos);

    protected override void DrawEdit(ref Rect pos)
    {
        base.DrawEdit(ref pos);
        pos.y += base.GetPropertyHeight();


        //draw filter sting field
        pos.height = SingleLineHeight;
        FilterString = EditorGUI.TextField(pos, "Filter Type", FilterString);



        //draw the type popup field
        pos.y += SingleLineHeight;

        lock (name)
        {
            SelectedIndex = EditorGUI.Popup(pos,"Field Type", SelectedIndex, FilteredTypes.Select(x => x.Name).ToArray());
        }
        pos.y += SingleLineHeight;


    }

    public override float GetPropertyHeight()
    {
        return base.GetPropertyHeight() + SingleLineHeight * (IsInEditMode ? 2 : 1);
    }
#endif

}

[Serializable]
public class EntryComponent_UnityObjectField : EntryComponent_ObjectField
{

    public new UnityEngine.Object Value
    {
        get { return base.Value as UnityEngine.Object; }
        set { base.Value = value; }
    }

    public override List<Type> GetAvailableTypes()
    {
        return (from t in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            where (t.IsPublic || t.IsNestedPublic) && !(t.IsAbstract && t.IsSealed) && !t.IsGenericType && typeof(UnityEngine.Object).IsAssignableFrom(t)
            select t).ToList();
    }

    protected override void DrawObjectField(ref Rect pos)
    {
        Value = EditorGUI.ObjectField(pos, new GUIContent(FieldName), Value, base.SelectedType, true);
    }

}


[Serializable]
public class EntryComponent_BasicsField : EntryComponent_ObjectField
{

    [SerializeField] private BasicField fieldViewer;
    private float height;

    public override List<Type> GetAvailableTypes()
    {
        return BasicField.GetBasicTypes();
    }

    protected override void DrawObjectField(ref Rect pos)
    {
        var oldPos = pos.y;

        pos.height = SingleLineHeight;
        Value= fieldViewer.DrawField(ref pos, FieldName, Value);

        height = pos.y - oldPos;

    }


    protected override void DrawEdit(ref Rect pos)
    {


        base.DrawEdit(ref pos);
        //calculate added height (pos.y - tempPos.y )
        var oldPos = pos.y;

        pos.height = SingleLineHeight;
        fieldViewer.DrawSelectFieldViewType(ref pos,SelectedType);
        fieldViewer.DrawSelectedTypeSetup(ref pos);


        height = pos.y - oldPos;



    }


    public override void Initialize(string componentName = "Object field name")
    {
        base.Initialize(componentName);
        fieldViewer = new BasicField();
    }

    public override float GetPropertyHeight()
    {
        return base.GetPropertyHeight() +height;
    }
}


[Serializable]
public class BasicField
{

    //statics----------------------------------------------------------------------
    static List<Type> _basicTypes;
    public static List<Type> GetBasicTypes()
    {
        if (_basicTypes != null)
            return _basicTypes;

        _basicTypes = new List<Type>
        {
            typeof(int),
            typeof(float),
            typeof(double),
            typeof(long),
            typeof(string),
            typeof(char),
            typeof(Vector2),
            typeof(Vector2Int),
            typeof(Vector3),
            typeof(Vector3Int),
            typeof(Vector4),
           
        };



        //enum
        //layer
        //mask
        //password
        //tag
        //
        return _basicTypes;
    }
  
    
    //constants--------------------------------------------------------------------
    private const string FieldName = " Field";
    private const string DelayedFieldName = " Delayed Field";
    private const string SliderName = " Slider";
    private const string PasswordName = " Password";

    private const string fieldViewTypeString = "Field View Type";

    //fields-----------------------------------------------------------------------
    private Dictionary<string, Func<Rect,string, object, object>> _fieldDrawer;
    public Dictionary<string, Func<Rect, string, object, object>> FieldDrawer
    {
        get
        {
            lock (SliderName)
            {
                if (_fieldDrawer == null) InitializeBasicDrawer();
                return _fieldDrawer;
            }
           
        }
    }


    //serialized fields------------------------------------------------------------
    [SerializeField] private int _fieldDrawTypeIndex;
    [SerializeField] private double _sliderMinVal;
    [SerializeField] private double _sliderMaxVal;
    [SerializeField] private string _selectedType;


    //constructors-----------------------------------------------------------------
    public BasicField()
    {
        InitializeBasicDrawer();
    }


    //private methods--------------------------------------------------------------
    private void InitializeBasicDrawer()
    {
        _fieldDrawer =  new Dictionary<string, Func<Rect,string, object, object>>();


        #region int

        //int field-----------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(int).Name + FieldName, 
            (pos, feldName, value)=>
            {
                if (!(value is int))
                    value = 0;
                return EditorGUI.IntField(pos, feldName, (int) value);
            });
       
        //int delayed field---------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(int).Name + DelayedFieldName,
            (pos, feldName, value) =>
            {
                if (!(value is int))
                    value = 0;
                return EditorGUI.DelayedIntField(pos, feldName, (int) value);
            });

        //int slider field----------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(int).Name + SliderName,
            (pos, feldName, value) =>
            {
                if (!(value is int))
                    value = 0;
                return (int)EditorGUI.Slider(pos, new GUIContent(feldName), (int)value, (int) _sliderMinVal,(int) _sliderMaxVal);
            });


        #endregion

        #region float

        //float field---------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(float).Name + FieldName,
            (pos, feldName, value) =>
            {
                if (!(value is float))
                    value = 0f;
                return EditorGUI.FloatField(pos, feldName, (float)value );
            });

        //float delayed field-------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(float).Name + DelayedFieldName,
            (pos, feldName, value) =>
            {
                if (!(value is float))
                    value = 0f;
                return EditorGUI.DelayedFloatField(pos, feldName, (float) value );
            });

        //float slider field--------- -----------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(float).Name + SliderName,
            (pos, feldName, value) =>
            {
                if (!(value is float))
                    value = 0f;
                return EditorGUI.Slider(pos, new GUIContent(feldName), (float) value , (float) _sliderMinVal,(float) _sliderMaxVal);
            });
        

        #endregion

        #region double

        //double field--------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(double).Name + FieldName,
            (pos, feldName, value) =>
            {
                if (!(value is double))
                    value = 0d;
                return EditorGUI.DoubleField(pos, feldName, (double) value);
            });

        //double delayed field------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(double).Name + DelayedFieldName,
            (pos, feldName, value) =>
            {
                if (!(value is double))
                    value = 0d;
                return EditorGUI.DelayedDoubleField(pos, feldName, (double) value);
            });
        

        #endregion

        #region long

        //long field---------------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(long).Name + FieldName,
            (pos, feldName, value) =>
            {
                if (!(value is long))
                    value = 0L;
                return EditorGUI.LongField(pos, feldName, (long) value );
            });

        

        #endregion

        #region string

        //string field--------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(string).Name + FieldName,
            (pos, feldName, value) =>
            {
                if (!(value is string))
                    value = "";
                return EditorGUI.TextField(pos, feldName, (string) value );
            });

        //string password field-----------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(string).Name + PasswordName,
            (pos, feldName, value) =>
            {
                if (!(value is string))
                    value = "";
                return EditorGUI.PasswordField(pos, feldName, (string) value);
            });

        

        #endregion

        #region char

        //char field----------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(char).Name + FieldName,
            (pos, feldName, value) =>
            {
                if (!(value is char))
                    value = ' ';
                return EditorGUI.TextField(pos, feldName, ((char) (value)).ToString())[0];
            });

        #endregion

        #region vectors

        //vector2 field----------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(Vector2).Name + FieldName,
            (pos, feldName, value) =>
            {
                if (!(value is Vector2))
                    value = Vector2.zero;
                return EditorGUI.Vector2Field(pos, new GUIContent(feldName), (Vector2) value );
            });

        //vector2 Int field---------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(Vector2Int).Name + FieldName,
            (pos, feldName, value) =>
            {
                if (!(value is Vector2Int))
                    value = Vector2Int.zero;
                return EditorGUI.Vector2IntField(pos, feldName, (Vector2Int) value);
            });

        //vector3 field-------------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(Vector3).Name + FieldName,
            (pos, feldName, value) =>
            {
                if (!(value is Vector3))
                    value = Vector3.zero;
                return EditorGUI.Vector3Field(pos, feldName, (Vector3) value);
            });

        //vector3 Int field---------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(Vector3Int).Name + FieldName,
            (pos, feldName, value) =>
            {
                if (!(value is Vector3Int))
                    value = Vector3Int.zero;
                return EditorGUI.Vector3IntField(pos, feldName, (Vector3Int) value);
            });

        //vector4 field-------------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(Vector4).Name + FieldName,
            (pos, feldName, value) =>
            {
                if (!(value is Vector4))
                    value = Vector4.zero;
                return EditorGUI.Vector4Field(pos, feldName, (Vector4) value);
            });
        

        #endregion



    }

    //public methods---------------------------------------------------------------
    public object DrawField(ref Rect pos, string fieldName, object value)
    {
        if (string.IsNullOrEmpty(_selectedType))
            throw new Exception("Please select the field view type in edit mode");

        pos.height = EntryComponent.SingleLineHeight;
        if (_selectedType.ToLower().Contains("vector"))
            pos.height *= 2;

        var temVal = this.FieldDrawer[_selectedType].Invoke(pos, fieldName, value);
        pos.y += pos.height;

        return temVal;

    }

    public void DrawSelectFieldViewType(ref Rect pos, Type t )
    {
        if (t == null) return;

        if (!GetBasicTypes().Contains(t))
            throw new ArgumentException("generic type is incomputable");



        var fieldDrawingTypes = this.FieldDrawer.Keys.Where(key => key.Contains(t.Name))
            .Select(key => key.Remove(0, key.IndexOf(' ')+1))
            .ToArray();

        if (fieldDrawingTypes.Length > 0)
        {
            pos.height = EntryComponent.SingleLineHeight;

            if (fieldDrawingTypes.Length <= _fieldDrawTypeIndex || _fieldDrawTypeIndex < 0)
                _fieldDrawTypeIndex = 0;

            _fieldDrawTypeIndex = EditorGUI.Popup(pos, fieldViewTypeString, _fieldDrawTypeIndex, fieldDrawingTypes);
            _selectedType = t.Name + " " + fieldDrawingTypes[_fieldDrawTypeIndex];
            pos.y += pos.height;
        }


    }

    public void DrawSelectedTypeSetup(ref Rect pos)
    {
        pos.height = EntryComponent.SingleLineHeight;

        if (!string.IsNullOrEmpty(_selectedType) && _selectedType.Contains(SliderName))
        {
            var val = EditorGUI.IntField(pos, "Min Val", (int) _sliderMinVal);
            if (val != (int) _sliderMinVal)
            {
                _sliderMinVal = val;
                if (_sliderMaxVal < _sliderMinVal) _sliderMaxVal = _sliderMinVal;

            }
            pos.y += pos.height;

            var val2 = EditorGUI.IntField(pos, "Max Val", (int) _sliderMaxVal);
            if (val2 != (int)_sliderMaxVal)
            {
                _sliderMaxVal = val2;
                if (_sliderMaxVal < _sliderMinVal) _sliderMinVal = _sliderMaxVal;

            }

            pos.y += pos.height;

        }
    }

    //-----------------------------------------------------------------------------





}
