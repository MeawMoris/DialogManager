using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[Serializable]
public class EntryComponent_UnityObjectField : EntryComponent_SelectTypeBase
{
    [SerializeField] private UnityEngine.Object _value;
    private Sprite s;
    public override object Value
    {
        get { return _value; }
        set { _value = (UnityEngine.Object) value; }
    }

    public override List<Type> GetAvailableTypes()
    {
        return (from t in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            where (t.IsPublic || t.IsNestedPublic) && !(t.IsAbstract && t.IsSealed) && !t.IsGenericType && typeof(UnityEngine.Object).IsAssignableFrom(t)
            select t).ToList();
    }
    public override object Clone()
    {
        var item =(EntryComponent_UnityObjectField)base.Clone();
        item._value = _value;
        return item;
    }


#if UNITY_EDITOR
    protected override void DrawObjectField(ref Rect pos)
    {
        if (Value != null && !Value.GetType().IsAssignableFrom(SelectedType) )
            Value = null;

        if (SelectedType == typeof(Texture) || SelectedType == typeof(Sprite))
            pos.height = SingleLineHeight * 3;
        Value = EditorGUI.ObjectField(pos, new GUIContent(FieldName), (UnityEngine.Object)Value, SelectedType, true);
    }

    public override float GetPropertyHeight()
    {
        if (!IsInEditMode && (SelectedType == typeof(Texture) || SelectedType == typeof(Sprite)))
            return base.GetPropertyHeight() + SingleLineHeight * 3;
        return base.GetPropertyHeight();
    }
#endif

}

