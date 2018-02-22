using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[Serializable, SelectableComponent("Collection")]
public class EntryComponent_Collection : EntryComponent_SelectTypeBase
{

    static readonly List<EntryComponent> EmptyList = new List<EntryComponent>();
    //-------------------------------------------------------------------------------------------------------------
    [SerializeField]private EntryComponentTemplate _template;
    [SerializeField] private CollectionComponent_CountLimiter _countLimiter;
    [SerializeField] bool _showListElemetsRemoveButton;
    internal ReorderableList _reorderableList;

    public EntryComponentTemplate Template { get { return _template; } }
    public CollectionComponent_CountLimiter CountLimiter
    {
        get
        {
            if(_countLimiter == null)
                _countLimiter = new CollectionComponent_CountLimiter(this);
            return _countLimiter;
        }
    }
    //-------------------------------------------------------------------------------------------------------------
    public override object Value
    {
        get { return (_template != null ? _template.ObserversList : EmptyList); }
        set { }
    }
    protected override string FieldTypeFieldString
    {
        get { return "Collection Type"; }
    }
    public override List<Type> GetAvailableTypes()
    {
        return (from t in Assembly.GetExecutingAssembly().GetTypes()
            where t.IsClass && t.IsPublic && !t.IsAbstract 
                  && typeof(EntryComponent).IsAssignableFrom(t) 
                  && t.GetCustomAttributes(true).Any(x=>x.GetType()== typeof(SelectableCollectionComponentAttribute))
            select t).ToList();
    }
    protected override bool ShowSearchField
    {
        get { return false; }
    }
    public override string FieldTypeLabel
    {
        get
        {
            return string.Format("{0} {1}", DefaultFieldTypeLabel,
                (SelectedType == null ? "" : string.Format("- {0}", SelectedType.Name.Split('_')[1])));
        }
    }

    public List<EntryComponent> ListOptions
    {
        get { return (_template != null ? _template.ObserversList : EmptyList); }
    }
    public bool ShowListElemetsRemoveButton
    {
        get { return _showListElemetsRemoveButton; }
        set { _showListElemetsRemoveButton = value; }
    }


    //-------------------------------------------------------------------------------------------------------------
    protected override void DrawEdit(ref Rect pos)
    {
        base.DrawEdit(ref pos);


        if (SelectedType != null)
        {
            if (_template == null)
                _template = new EntryComponentTemplate(SelectedType, Holder);

            if (_reorderableList == null)
                InitializeReorderableList();

            //drawLimiter
            CountLimiter.DrawEdit(ref pos);


            //draw template
            _template.SetTemplateType(SelectedType);
            _template.TemplateInstance.IsInEditMode = true;
            _template.TemplateInstance.ShowEditFieldName = false;
            _template.TemplateInstance.ShowFieldTypeLabel = false;
            pos.height = _template.TemplateInstance.GetPropertyHeight();
            DrawQuad(pos, 100, 0, 255, .2f);
            _template.TemplateInstance.DrawView(ref pos);
        }
    }

    protected override void DrawObjectField(ref Rect pos)
    {
        if (_reorderableList == null)       
            InitializeReorderableList();

        if(_countLimiter != null)
            CountLimiter.ApplyLimits();

        pos.y += 3;
        _reorderableList.DoList(pos);

    }
    public override float GetPropertyHeight()
    {
        float addedHeight = 0;
        if (IsInEditMode && _countLimiter!= null)
            addedHeight += CountLimiter.GetEditProperyHeight();
        if (!IsInEditMode && SelectedType != null && _reorderableList != null)
            addedHeight += _reorderableList.GetHeight() + 5;
        if (IsInEditMode && SelectedType != null && _template != null)
            addedHeight += _template.TemplateInstance.GetPropertyHeight();

        return base.GetPropertyHeight() + addedHeight;
    }


    public override void CloneTo(EntryComponent other)
    {
        base.CloneTo(other);
        var instance = (EntryComponent_Collection)other;

        if (_template!= null)
            instance._template = new EntryComponentTemplate(_template);

        if (_countLimiter != null)
            instance._countLimiter = new CollectionComponent_CountLimiter(instance, CountLimiter);

        instance._showListElemetsRemoveButton = _showListElemetsRemoveButton;
    }
    protected override void Initialize(string componentName = "Object field name")
    {
        base.Initialize(componentName);
        ShowListElemetsRemoveButton = true;
        _countLimiter = null;
        _template = null;
    }
    //-------------------------------------------------------------------------------------------------------------


    private void InitializeReorderableList()
    {
        _reorderableList = new ReorderableList(_template.ObserversList, SelectedType,true,false,true,false);
        _reorderableList.onAddCallback += OnAddComponentClick;
        _reorderableList.drawElementCallback += DrawElementCallback;
        _reorderableList.elementHeightCallback += ElementHeightCallback;
        _reorderableList.drawElementBackgroundCallback += DrawElementBackgroundCallback;
        _reorderableList.drawHeaderCallback+= DrawHeaderCallback;
        _reorderableList.displayRemove = false;
    }

    private void DrawHeaderCallback(Rect rect)
    {
        EditorGUI.LabelField(rect,FieldName);
    }

    private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        if (index >= _reorderableList.list.Count || index < 0)
            return;

        //set field width
        var pos = rect;

        if (ShowListElemetsRemoveButton)
            pos.width -= EditorGUIUtility.singleLineHeight;

        //draw field
        var entryComponent = ((EntryComponent) _reorderableList.list[index]);
        entryComponent.FieldName =string.Format("index {0}", index);
        entryComponent.DrawView(ref pos);

        if (ShowListElemetsRemoveButton)
        {
            //calculate remove button pos
            pos.height = pos.width = EditorGUIUtility.singleLineHeight;
            pos.x = rect.x + rect.width - EditorGUIUtility.singleLineHeight;
            pos.y = rect.y + entryComponent.GetPropertyHeight() / 2 - pos.height / 2;

            //on remove button pressed

            if (GUI.Button(pos, "-"))
                _reorderableList.list.RemoveAt(index);
        }


    }
    private void DrawElementBackgroundCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        DrawQuad(rect, 100, 0, 255,.2f);
    }


    private void OnAddComponentClick(ReorderableList list)
    {
        if (_template != null)
        {
            _template.AddObserver();
            _template.ObserversList[_template.ObserversList.Count - 1].IsInEditMode = false;
            _template.ObserversList[_template.ObserversList.Count - 1].ShowFieldTypeLabel = false;
        }
       
    } 
    private float ElementHeightCallback(int index)
    {
        if (index >= _reorderableList.list.Count || index < 0)
            return 0;
        return ((EntryComponent)_reorderableList.list[index]).GetPropertyHeight();
    }
    //-------------------------------------------------------------------------------------------------------------
}

[Serializable]
public enum CollectionLimiterType
{
    None,
    FixedCount,
    Range,
    Locked
}

[Serializable]
public class CollectionComponent_CountLimiter
{
    [SerializeField] private EntryComponent_Collection _collection;
    [SerializeField] private bool _showAddButton;
    [SerializeField] private bool _showRemoveButton;
    [SerializeField] private CollectionLimiterType _countType;
    [SerializeField] private int _minCount;
    [SerializeField] private int _maxCount;

    public CollectionComponent_CountLimiter(EntryComponent_Collection collection)
    {
        if (collection == null) throw new ArgumentNullException("collection");
        this._collection = collection;
        _showRemoveButton = collection.ShowListElemetsRemoveButton;
    }
    public CollectionComponent_CountLimiter(EntryComponent_Collection collection,CollectionComponent_CountLimiter other)
    {
        if (collection == null) throw new ArgumentNullException("collection");
        _collection = collection;
        _minCount= other._minCount;
        _maxCount= other._maxCount;
        _countType = other._countType;
        _showAddButton = other._showAddButton;
        _showRemoveButton = collection.ShowListElemetsRemoveButton;

    }

    public void ApplyLimits()
    {
        switch (_countType)
        {
            case CollectionLimiterType.None:
                _showAddButton = true;
                _showRemoveButton= true;
                break;
            case CollectionLimiterType.FixedCount:
                _showAddButton = false;
                _showRemoveButton = false;

                if (_collection.Template == null)
                    return;

                for (int i = _minCount; i < _collection.Template.ObserversList.Count; i++)              
                    _collection.Template.ObserversList.RemoveAt(_collection.Template.ObserversList.Count-1);
                for (int i = _collection.Template.ObserversList.Count; i < _minCount; i++)
                    _collection._reorderableList.onAddCallback.Invoke(_collection._reorderableList);

                break;
            case CollectionLimiterType.Range:
                _showRemoveButton = _collection.Template.ObserversList.Count > _minCount;
                _showAddButton = _collection.Template.ObserversList.Count < _maxCount;
                if (_collection.Template == null)
                    return;

                for (int i = _maxCount; i < _collection.Template.ObserversList.Count; i++)
                    _collection.Template.ObserversList.RemoveAt(_collection.Template.ObserversList.Count - 1);

                for (int i = _collection.Template.ObserversList.Count; i < _minCount; i++)
                    _collection._reorderableList.onAddCallback.Invoke(_collection._reorderableList);

                break;
            case CollectionLimiterType.Locked:
                _showAddButton = false;
                _showRemoveButton = false;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _collection._reorderableList.displayAdd = _showAddButton;
        _collection.ShowListElemetsRemoveButton = _showRemoveButton;
    }

    public void DrawEdit(ref Rect pos)
    {
        Show_LimitCountField_SelectLimitType(ref pos);
        Show_LimitCountField_LimitTypeOptionSetting(ref pos);
    }

    private void Show_LimitCountField_SelectLimitType(ref Rect pos)
    {
        pos.height = EntryComponent.SingleLineHeight;
        var temp = (CollectionLimiterType) EditorGUI.EnumPopup(pos, "Elements Count Limit", _countType);
        if (temp != _countType)
        {
            _countType = temp;
            OnTypeChange();
        }
        pos.y += pos.height;
    }
    void OnTypeChange()
    {
        _maxCount = _minCount = 0;
        ApplyLimits();

        if (_collection.OnEditModeModified != null)
            _collection.OnEditModeModified();
    }

    private void Show_LimitCountField_LimitTypeOptionSetting(ref Rect pos)
    {
        pos.height = EntryComponent.SingleLineHeight;

        switch (_countType)
        {

            case CollectionLimiterType.FixedCount:
                var minTemp = EditorGUI.IntField(pos, "Cell Count", _minCount);
                if (_minCount != minTemp)
                {
                    if (minTemp < 0) minTemp = 0;
                    _minCount = minTemp;
                    if (_collection.OnEditModeModified != null)
                        _collection.OnEditModeModified();
                }
                pos.y += pos.height;
                break;
            case CollectionLimiterType.Range:
                var minTemp2 = EditorGUI.IntField(pos,"Min Cell Count", _minCount);
                if (_minCount != minTemp2)
                {
                    if (minTemp2 < 0) minTemp2 = 0;
                    _minCount = minTemp2;
                    if (_maxCount < _minCount)
                        _maxCount = _minCount;

                    if (_collection.OnEditModeModified != null)
                        _collection.OnEditModeModified();
                }
                pos.y += pos.height;

                var maxTemp = EditorGUI.IntField(pos, "Max Cell Count", _maxCount);
                if (_maxCount != maxTemp)
                {
                    _maxCount = maxTemp;
                    if (_maxCount < _minCount)
                        _minCount = maxTemp;

                    if (_collection.OnEditModeModified != null)
                        _collection.OnEditModeModified();
                }
                pos.y += pos.height;

                break;

            case CollectionLimiterType.Locked:
            case CollectionLimiterType.None:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public float GetEditProperyHeight()
    {
        switch (_countType)
        {

            case CollectionLimiterType.FixedCount:
                return EntryComponent.SingleLineHeight*2;
            case CollectionLimiterType.Range:
                return EntryComponent.SingleLineHeight * 3;

            case CollectionLimiterType.Locked:
            case CollectionLimiterType.None:
                return EntryComponent.SingleLineHeight;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}