using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[Serializable]
public class EntryComponent_BasicField : EntryComponent_SelectTypeBase
{


    [SerializeField]private BasicFieldData fieldData;
    protected object _value;


    public override object Value
    {
        get { return FieldData.GetValue(SelectedType); }
        set
        {
            if (OnViewModeModified != null && !FieldData.GetValue(SelectedType).Equals(value))
            {
                FieldData.SetVal(value);
                OnViewModeModified();
                return;
            }

            FieldData.SetVal(value);
        }
    }
    protected override bool ShowSearchField
    {
        get { return false; }
    }

    public BasicFieldData FieldData
    {
        get { return fieldData?? (fieldData = new BasicFieldData()); }
    }



    public override List<Type> GetAvailableTypes()
    {
        return BasicFieldViewer.GetBasicTypes();
    }
    public override void Initialize(string componentName = "Object field name")
    {
        base.Initialize(componentName);
        fieldViewer = new BasicFieldViewer();
    }
    public override object Clone()
    {
        EntryComponent_BasicField component = (EntryComponent_BasicField) base.Clone();
        component.fieldData = new BasicFieldData(FieldData);
        component.fieldViewer = new BasicFieldViewer(FieldViewer);
        return component;
    }
    public override void CloneTo(EntryComponent other)
    {
        base.CloneTo(other);
        var otherInst = other as EntryComponent_BasicField;
        if (otherInst == null)
            throw new ArgumentException("types are not the same");
        otherInst.fieldData = new BasicFieldData(fieldData);
        otherInst.fieldViewer = new BasicFieldViewer(fieldViewer);
    }




#if UNITY_EDITOR
    [SerializeField] private BasicFieldViewer fieldViewer;
    private float height;

    public BasicFieldViewer FieldViewer
    {
        get { return fieldViewer?? (fieldViewer = new BasicFieldViewer()); }
    }


    protected override void DrawObjectField(ref Rect pos)
    {
        var oldPos = pos.y;

        pos.height = SingleLineHeight;
        Value= FieldViewer.DrawField(ref pos, FieldName, Value);

        height = pos.y - oldPos ;

    }
    protected override void DrawEdit(ref Rect pos)
    {


        base.DrawEdit(ref pos);
        //calculate added height (pos.y - tempPos.y )
        var oldPos = pos.y;

        pos.height = SingleLineHeight;
        FieldViewer.DrawSelectFieldViewType(ref pos,SelectedType,OnEditModeModified);
        FieldViewer.DrawSelectedTypeSetup(ref pos, OnEditModeModified);


        height = pos.y - oldPos;



    }

    public override float GetPropertyHeight()
    {
        return base.GetPropertyHeight() +height;
    }
#endif

}


#if UNITY_EDITOR

[Serializable]
public class BasicFieldViewer
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
        _basicTypes.Sort( (type, type1) =>String.Compare(type.Name, type1.Name, StringComparison.Ordinal));


        return _basicTypes;
    }


    //constants--------------------------------------------------------------------
    private const string FieldName = " Field";
    private const string DelayedFieldName = " Delayed Field";
    private const string SliderName = " Slider";
    private const string PasswordName = " Password";
    private const string TextAreaName = " TextArea";
    private const int TextAreaLinesNum = 5;

    private const string fieldViewTypeString = "Field View Type";

    //fields-----------------------------------------------------------------------
    private Dictionary<string, Func<Rect, string, object, object>> _fieldDrawer;
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
    public BasicFieldViewer()
    {
        InitializeBasicDrawer();
    }
    public BasicFieldViewer(BasicFieldViewer other)
    {
        InitializeBasicDrawer();
        _fieldDrawTypeIndex = other._fieldDrawTypeIndex;
        _sliderMinVal = other._sliderMinVal;
        _sliderMaxVal = other._sliderMaxVal;
        _selectedType = other._selectedType;
    }


    //private methods--------------------------------------------------------------
    private void InitializeBasicDrawer()
    {
        _fieldDrawer = new Dictionary<string, Func<Rect, string, object, object>>();


        #region int

        //int field-----------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(int).Name + FieldName,
            (pos, fieldName, value) =>
            {
                if (!(value is int))
                    value = 0;
                return EditorGUI.IntField(pos, fieldName, (int)value);
            });

        //int delayed field---------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(int).Name + DelayedFieldName,
            (pos, fieldName, value) =>
            {
                if (!(value is int))
                    value = 0;
                return EditorGUI.DelayedIntField(pos, fieldName, (int)value);
            });

        //int slider field----------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(int).Name + SliderName,
            (pos, fieldName, value) =>
            {
                if (!(value is int))
                    value = 0;
                return (int)EditorGUI.Slider(pos, new GUIContent(fieldName), (int)value, (int)_sliderMinVal, (int)_sliderMaxVal);
            });


        #endregion

        #region float

        //float field---------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(float).Name + FieldName,
            (pos, fieldName, value) =>
            {
                if (!(value is float))
                    value = 0f;
                return EditorGUI.FloatField(pos, fieldName, (float)value);
            });

        //float delayed field-------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(float).Name + DelayedFieldName,
            (pos, fieldName, value) =>
            {
                if (!(value is float))
                    value = 0f;
                return EditorGUI.DelayedFloatField(pos, fieldName, (float)value);
            });

        //float slider field--------- -----------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(float).Name + SliderName,
            (pos, fieldName, value) =>
            {
                if (!(value is float))
                    value = 0f;
                return EditorGUI.Slider(pos, new GUIContent(fieldName), (float)value, (float)_sliderMinVal, (float)_sliderMaxVal);
            });


        #endregion

        #region double

        //double field--------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(double).Name + FieldName,
            (pos, fieldName, value) =>
            {
                if (!(value is double))
                    value = 0d;
                return EditorGUI.DoubleField(pos, fieldName, (double)value);
            });

        //double delayed field------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(double).Name + DelayedFieldName,
            (pos, fieldName, value) =>
            {
                if (!(value is double))
                    value = 0d;
                return EditorGUI.DelayedDoubleField(pos, fieldName, (double)value);
            });


        #endregion

        #region long

        //long field---------------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(long).Name + FieldName,
            (pos, fieldName, value) =>
            {
                if (!(value is long))
                    value = 0L;
                return EditorGUI.LongField(pos, fieldName, (long)value);
            });



        #endregion

        #region string

        //string field--------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(string).Name + FieldName,
            (pos, fieldName, value) =>
            {
                if (!(value is string))
                    value = "";
                return EditorGUI.TextField(pos, fieldName, (string)value);
            });

        //string field--------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(string).Name + TextAreaName,
            (pos, fieldName, value) =>
            {
                if (!(value is string))
                    value = "";
                Rect temp = pos;
                temp.height = EntryComponent.SingleLineHeight;
                EditorGUI.LabelField(temp, fieldName);
                temp.y += temp.height;
                temp.height = pos.height - temp.height;
                return EditorGUI.TextArea(temp, (string)value, EditorStyles.textArea);
            });

        //string password field-----------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(string).Name + PasswordName,
            (pos, fieldName, value) =>
            {
                if (!(value is string))
                    value = "";
                return EditorGUI.PasswordField(pos, fieldName, (string)value);
            });



        #endregion

        #region char

        //char field----------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(char).Name + FieldName,
            (pos, fieldName, value) =>
            {
                if (!(value is char))
                    value = ' ';
                return EditorGUI.TextField(pos, fieldName, ((char)(value)).ToString())[0];
            });

        #endregion

        #region vectors

        //vector2 field----------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(Vector2).Name + FieldName,
            (pos, fieldName, value) =>
            {
                if (!(value is Vector2))
                    value = Vector2.zero;
                return EditorGUI.Vector2Field(pos, new GUIContent(fieldName), (Vector2)value);
            });

        //vector2 Int field---------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(Vector2Int).Name + FieldName,
            (pos, fieldName, value) =>
            {
                if (!(value is Vector2Int))
                    value = Vector2Int.zero;
                return EditorGUI.Vector2IntField(pos, fieldName, (Vector2Int)value);
            });

        //vector3 field-------------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(Vector3).Name + FieldName,
            (pos, fieldName, value) =>
            {
                if (!(value is Vector3))
                    value = Vector3.zero;
                return EditorGUI.Vector3Field(pos, fieldName, (Vector3)value);
            });

        //vector3 Int field---------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(Vector3Int).Name + FieldName,
            (pos, fieldName, value) =>
            {
                if (!(value is Vector3Int))
                    value = Vector3Int.zero;
                return EditorGUI.Vector3IntField(pos, fieldName, (Vector3Int)value);
            });

        //vector4 field-------------------------------------------------------------------------------------------------
        _fieldDrawer.Add(typeof(Vector4).Name + FieldName,
            (pos, fieldName, value) =>
            {
                if (!(value is Vector4))
                    value = Vector4.zero;
                return EditorGUI.Vector4Field(pos, fieldName, (Vector4)value);
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

        else if(_selectedType.Contains(TextAreaName))
            pos.height *= TextAreaLinesNum;


        var temVal = FieldDrawer[_selectedType].Invoke(pos, fieldName, value);
        pos.y += pos.height;


        return temVal;

    }

    public void DrawSelectFieldViewType(ref Rect pos, Type t, Action onEditModified = null)
    {
        if (t == null) return;

        if (!GetBasicTypes().Contains(t))
            throw new ArgumentException("generic type is incomputable");


        //get all the selected field view types
        var fieldDrawingTypes = this.FieldDrawer.Keys.Where(key => key.Contains(t.Name))
            .Select(key => key.Remove(0, key.IndexOf(' ') + 1))
            .ToArray();



        if (fieldDrawingTypes.Length > 0)
        {
            pos.height = EntryComponent.SingleLineHeight;

            //if selected type is out of array range
            if (fieldDrawingTypes.Length <= _fieldDrawTypeIndex || _fieldDrawTypeIndex < 0)
                _fieldDrawTypeIndex = 0;

            //draw select type viewer pop-up
             _fieldDrawTypeIndex = EditorGUI.Popup(pos, fieldViewTypeString, _fieldDrawTypeIndex, fieldDrawingTypes);



            var newSelectedType = t.Name + " " + fieldDrawingTypes[_fieldDrawTypeIndex];
            if (_selectedType == null || !_selectedType.Equals(newSelectedType))
            {
                _selectedType = newSelectedType;

                if (onEditModified!=null)
                    onEditModified();

            }
            pos.y += pos.height;            
            

        }


    }

    public void DrawSelectedTypeSetup(ref Rect pos, Action onEditModified = null)
    {
        pos.height = EntryComponent.SingleLineHeight;

        if (!string.IsNullOrEmpty(_selectedType) && _selectedType.Contains(SliderName))
        {
            bool valueChanged = false;

            var val = EditorGUI.IntField(pos, "Min Val", (int)_sliderMinVal);
            if (val != (int)_sliderMinVal)
            {
                _sliderMinVal = val;
                if (_sliderMaxVal < _sliderMinVal) _sliderMaxVal = _sliderMinVal;
                valueChanged = true;
            }
            pos.y += pos.height;

            var val2 = EditorGUI.IntField(pos, "Max Val", (int)_sliderMaxVal);
            if (val2 != (int)_sliderMaxVal)
            {
                _sliderMaxVal = val2;
                if (_sliderMaxVal < _sliderMinVal) _sliderMinVal = _sliderMaxVal;
                valueChanged = true;
            }

            pos.y += pos.height;


            if (valueChanged && onEditModified != null)
                onEditModified();
        }
    }


    //-----------------------------------------------------------------------------
}
#endif


[Serializable]
public class BasicFieldData
{
    [SerializeField] private double _doubleVal ;//holds float and double values
    [SerializeField] private long _longVal;//holds int and long values

    [SerializeField] private char _charVal;
    [SerializeField] private string _stringVal;

    [SerializeField] private Vector3Int _vector3IntVal;
    [SerializeField] private Vector4 _vector4Val;//holds vector2, vector3 and vector4 values


    public BasicFieldData(BasicFieldData other)
    {

        this._doubleVal = other._doubleVal;
        this._longVal = other._longVal;
        this._charVal = other._charVal;
        this._stringVal = other._stringVal;
        this._vector3IntVal = other._vector3IntVal;
        this._vector4Val = other._vector4Val;
    }

    public T GetVal<T>()
    {
        return (T)GetValue(typeof(T));
    }

    public BasicFieldData()
    {
        _doubleVal = 0;
        _longVal = 0;
        _charVal = ' ';
        _stringVal = "";
        _vector4Val = Vector4.zero;
        _vector3IntVal = Vector3Int.zero;
    }
    public object GetValue(Type objType)
    {
        if (objType == typeof(int))
            return (_longVal > int.MaxValue) ? int.MaxValue : Convert.ToInt32(_longVal);
        if (objType == typeof(long))
            return _longVal;


        if (objType == typeof(float))
            return _doubleVal > float.MaxValue ? float.MaxValue : Convert.ToSingle(_doubleVal);
        if (objType == typeof(double))
            return _doubleVal;


        if (objType == typeof(string))
            return _stringVal;
        if (objType == typeof(char))
            return string.IsNullOrEmpty(_stringVal) ? ' ' : _stringVal[0];

        if (objType == typeof(Vector3))
            return (Vector3)_vector4Val;

        if (objType == typeof(Vector2))
            return new Vector2(_vector4Val.x, _vector4Val.y);

        if (objType == typeof(Vector3Int))
            return _vector3IntVal;

        if (objType == typeof(Vector2Int))
            return new Vector2Int(_vector3IntVal.x, _vector3IntVal.y);

        if (objType == typeof(Vector4))
            return _vector4Val;

        throw new ArgumentException("unsupported type");
    }


    public void SetVal(object val)
    {
        if (val is double || val is long || val is int || val is float)
        {
            _longVal = Convert.ToInt64(val);
            _doubleVal = Convert.ToSingle(val);
        }
        else if (val is string || val is char)
        {
            _stringVal = Convert.ToString(val);
        }
        else if (val is Vector3)
        {
            var vector3Val = (Vector3)val;
            _vector3IntVal.x = (int)(_vector4Val.x =  vector3Val.x);
            _vector3IntVal.y = (int)(_vector4Val.y =  vector3Val.y);
            _vector3IntVal.z = (int)(_vector4Val.z =  vector3Val.z);
        }
        else if (val is Vector2)
        {
            var vector2Val = (Vector2)val;
            _vector3IntVal.x = (int)(_vector4Val.x = vector2Val.x);
            _vector3IntVal.y = (int)(_vector4Val.y = vector2Val.y);
        }
        else if (val is Vector3Int)
        {
            _vector3IntVal = (Vector3Int)val;
            _vector4Val.x = _vector3IntVal.x = _vector3IntVal.x;
            _vector4Val.y = _vector3IntVal.y = _vector3IntVal.y;
            _vector4Val.y = _vector3IntVal.z = _vector3IntVal.z;
        }
        else if (val is Vector2Int)
        {
            var vector2Int = (Vector2Int)val;
            _vector4Val.x = _vector3IntVal.x = vector2Int.x;
            _vector4Val.y = _vector3IntVal.y = vector2Int.y;
        }
        else if (val is Vector4)
        {
            _vector4Val = (Vector4)val;
            _vector3IntVal.x = (int)(_vector4Val.x);
            _vector3IntVal.y = (int)(_vector4Val.y);
            _vector3IntVal.z = (int)(_vector4Val.z);
        }
    }


}