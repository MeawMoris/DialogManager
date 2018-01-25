using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(DialogParticipant))]
public class DialogParticipantDrawer : PropertyDrawer
{
    private Vector2 scrollerPos;
    private float totalHeight;
    private ReorderableList spritesListViewer;
    private List<string> _popupNames = new List<string>();



    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        //base.OnGUI(position, property, label);
        var pos = position;

        totalHeight = base.GetPropertyHeight(property, label);

        if (spritesListViewer == null)
            InitializeReorderableList(property);


        //name field------------------------------------------------------------------

        pos.height = EditorGUIUtility.singleLineHeight;


        var nameProperty = property.FindPropertyRelative("_name");

        if (string.IsNullOrEmpty(nameProperty.stringValue))
            nameProperty.stringValue = DialogManager.GetInstance().AnonymousParticipant.Name;//todo search if name is used or not

        nameProperty.stringValue= EditorGUI.TextField(pos, nameProperty.displayName, nameProperty.stringValue);
      
        //sprites--field--------------------------------------------------------------
        pos.y += EditorGUIUtility.singleLineHeight * 2;
        pos.height = spritesListViewer.GetHeight() + EditorGUIUtility.singleLineHeight * 2;
        totalHeight += pos.height;

        RemoveDuplicates(property);
        DragSpritesToList(property,pos);
        spritesListViewer.DoList(pos);


        //default sprite field--------------------------------------------------------
        pos.y += spritesListViewer.GetHeight()+ EditorGUIUtility.singleLineHeight;
        pos.height = EditorGUIUtility.singleLineHeight;
        totalHeight += pos.height;


        //  pos.y += position.y+pos.height;
        var defaultSpriteIndexProperty = property.FindPropertyRelative("_defaultSpriteIndex");

        _popupNames= EditorParticipantsUtility.GetParticipantSpriteNames(property);

        defaultSpriteIndexProperty.intValue = EditorGUI.Popup(pos, "Default Sprite", defaultSpriteIndexProperty.intValue, _popupNames.ToArray());

    }


    private void InitializeReorderableList(SerializedProperty property)
    {
        //var list = property.FindPropertyRelative("_sprites").arra as List<Sprite>;
        var listt = property.FindPropertyRelative("_sprites");
        spritesListViewer = new ReorderableList(property.serializedObject, listt, false, true, true, true);

        spritesListViewer.drawHeaderCallback = rect => EditorGUI.LabelField(rect, listt.displayName); 
        spritesListViewer.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = listt.GetArrayElementAtIndex(index);
                var sprite = element.objectReferenceValue as Sprite;
                // EditorGUI.DrawPreviewTexture(new Rect(rect.x, rect.y, rect.width*0.75f, rect.height), sprite, );
                // GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width * 0.75f, rect.height), sprite.texture, ScaleMode.ScaleToFit);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width , EditorGUIUtility.singleLineHeight), element);
            };


   
        spritesListViewer.onAddCallback = x =>
        {
            listt.InsertArrayElementAtIndex(listt.arraySize);
            listt.GetArrayElementAtIndex(listt.arraySize-1).objectReferenceValue = null;
            RemoveDuplicates(property);


        };
        spritesListViewer.onRemoveCallback = list =>
        {
            listt.GetArrayElementAtIndex(list.index).objectReferenceValue=null;
            listt.DeleteArrayElementAtIndex(list.index);

        };

    }




    void DragSpritesToList(SerializedProperty property, Rect dropArea)
    {
        Event evt = Event.current;
        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    return;

                if (!DragAndDrop.objectReferences.All(x=>x is Sprite))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {

                    DragAndDrop.AcceptDrag();
                    foreach (Sprite dragged_object in DragAndDrop.objectReferences)
                    {
                        InsertElement(property, dragged_object);

                    }
                    RemoveDuplicates(property);
                    
                }
                break;
        }
    }
    private void InsertElement(SerializedProperty property, Sprite dragged_object)
    {
        var listt = property.FindPropertyRelative("_sprites");
        var count = listt.arraySize;
        listt.InsertArrayElementAtIndex(count);
        listt.GetArrayElementAtIndex(count).objectReferenceValue = dragged_object;
    }
    private void RemoveDuplicates(SerializedProperty property)
    {
        var listt = property.FindPropertyRelative("_sprites");
        var count = listt.arraySize;

        IList filteredRows = new ArrayList();
        for (int i = 0; i < count; i++)       
            filteredRows.Add(listt.GetArrayElementAtIndex(i).objectReferenceValue);
        

        for (var i = 0; i < count; i++)
        {
            filteredRows.RemoveAt(0);
            var CueewnrElement = listt.GetArrayElementAtIndex(i);
            if (filteredRows.Contains(CueewnrElement.objectReferenceValue)
                ||(CueewnrElement.objectReferenceValue == null && i != count - 1))
            {
                CueewnrElement.objectReferenceValue = null;
                listt.DeleteArrayElementAtIndex(i);
                i--;
                count--;
                
            }

        }

    }


  



    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // return base.GetPropertyHeight(property, label);
        return totalHeight;
    }



}