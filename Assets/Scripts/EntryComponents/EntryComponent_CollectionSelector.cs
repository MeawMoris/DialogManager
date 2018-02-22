using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable, SelectableComponent("Collection Selector"), SelectableCollectionComponent]
public class EntryComponent_CollectionSelector : EntryComponent
{
    [SerializeField] private EntryComponent_Collection _selectedCollection;
    [SerializeField] private EntryComponent _selectedCollectionItem;


    public List<EntryComponent_Collection> AvailableCollections
    {
        get
        {
            return  Holder.Componets.OfType<EntryComponent_Collection>().ToList();
        }
    }

    protected override void Draw(ref Rect pos)
    {

        if (CheckIfComponentInitialized(ref pos)) return;


        CheckIfCollectionRemoved(AvailableCollections);


        pos.height = SingleLineHeight;

        if (_selectedCollection == null)       
            EditorGUI.LabelField(pos, "please select a collection in the edit mode");
        
        else
        {
            int selectedItemIndex = _selectedCollection.ListOptions.IndexOf(_selectedCollectionItem);
            if (selectedItemIndex == -1)
                _selectedCollectionItem = null;


            var newSelectedItemIndex = EditorGUI.Popup(pos,FieldName , selectedItemIndex, _selectedCollection.ListOptions.Select(x => x.FieldName).ToArray());

            if (newSelectedItemIndex != -1 && newSelectedItemIndex != selectedItemIndex)
                _selectedCollectionItem = _selectedCollection.ListOptions[newSelectedItemIndex];

        }

        pos.y += pos.height;

    }

    protected override void DrawEdit(ref Rect pos)
    {
        base.DrawEdit(ref pos);
        pos.height = SingleLineHeight;

        if (CheckIfComponentInitialized(ref pos)) return;


        var availableCollections = AvailableCollections;

        if (availableCollections.Count == 0)
        {
            EditorGUI.LabelField(pos, "no collection found to choose from.");
            _selectedCollection = null;
        }

        else
        {
            int selectedCollectionIndex = availableCollections.IndexOf(_selectedCollection);
            if (selectedCollectionIndex == -1)
                _selectedCollection = null;

            var newSelectedCollectionIndex= EditorGUI.Popup(pos,"Select Collection", selectedCollectionIndex,availableCollections.Select(x => x.FieldName).ToArray());
            if (newSelectedCollectionIndex != -1 && newSelectedCollectionIndex != selectedCollectionIndex)
                _selectedCollection =  availableCollections[newSelectedCollectionIndex];
        }


        
    }

    private bool CheckIfComponentInitialized(ref Rect pos)
    {
        if (Holder == null)
        {
            EditorGUI.LabelField(pos, "Please initialize the component");
            pos.y += pos.height;
            return true;
        }
        return false;
    }
    private void CheckIfCollectionRemoved(List<EntryComponent_Collection> collectionsList)
    {
        int selectedCollectionIndex = collectionsList.IndexOf(_selectedCollection);
        if (selectedCollectionIndex == -1)
            _selectedCollection = null;
    }

    public override void CloneTo(EntryComponent other)
    {
        base.CloneTo(other);
        var instance = other as EntryComponent_CollectionSelector;
        instance._selectedCollection = _selectedCollection;
        instance._selectedCollectionItem= _selectedCollectionItem;
    }

    public override float GetPropertyHeight()
    {
        return base.GetPropertyHeight()+ SingleLineHeight;
    }
}