using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


public class DialogComponent : MonoBehaviour
{
    // [SerializeField]
    // private TextAsset _xmlDialogFile;

    // [SerializeField]
    //private Dialog _info;

    //  public image
    void Update()
    {
    }
}




[Serializable]
public class Dialog
{
    [SerializeField] DialogSettings _dialogSettings;
    [SerializeField]  ParticipantSelectorList _participantSelectors;
    [SerializeField]  ParticipantImageSettingsList _participantsImageSettings;
    [SerializeField]  List<DialogEntry> _dialogEntries;

    public Dialog(ParticipantSelectorList participantSelectors)
    {      
    }
    //dialog view window:
    /*
     * --add participants
     * --participants image settings
     * --dialog entries 
     */



/*    /// <summary>
    /// starts a dialog if none is stated.
    /// 
    /// returns false id another dialog is taking place and "multi-dialog" feature is disabled
    ///  or if dialogs are disabled.
    /// </summary>
    /// <returns>returns true if the dialog started, otherwise returns false</returns>
    public bool StartDialog()
    {
        //todo recieve dialogData
        //todo return false if a multi dialog is displaying
        //todo show multiple dialogs if the feature is enabled
        return false;
    }




    //todo implement events invokers 
    //todo invoke events at the right moment
    public event EventHandler OnDialogStart;
    public event EventHandler OnDialogEnd;
    public event EventHandler OnSlideChange;
    public bool IsDialogRunnig { get; private set; }*/



}

[CustomPropertyDrawer(typeof(Dialog))]
public class DialogDrawer : PropertyDrawer
{
    private float height = EditorGUIUtility.singleLineHeight*3;
    private List<DialogParticipant> _allParticipants;
    private bool participantSelectorsFoldout;
    private bool imageSettingFoldout;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        _allParticipants = DialogManager.GetInstance().GetAllParticipants();
        //--------------------------------------------------------------------------------------------------------
        var _dialogSettings = property.FindPropertyRelative("_dialogSettings");
        var _participantSelectors = property.FindPropertyRelative("_participantSelectors");
        var _participantsImageSettings = property.FindPropertyRelative("_participantsImageSettings");
        var _dialogEntries = property.FindPropertyRelative("_dialogEntries");
        //----------------------------------------------------------------------------------------
        var rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;
        height = 0;
        //--------------------------------------------------------------------------------------------------------
        DrawDialogSettings(_dialogSettings,ref rect);
        //--------------------------------------------------------------------------------------------------------
        participantSelectorsFoldout = EditorGUI.Foldout(rect, participantSelectorsFoldout,"Select Participants In this Dialog");

        rect.y += EditorGUIUtility.singleLineHeight;
        //height += EditorGUIUtility.singleLineHeight;
        if(participantSelectorsFoldout)
            DrawParticipantSelectors(_participantSelectors, ref rect);
        else height += EditorGUIUtility.singleLineHeight;
        //--------------------------------------------------------------------------------------------------------
        imageSettingFoldout = EditorGUI.Foldout(rect, imageSettingFoldout, "Select Participants Image Settings");

        rect.y += EditorGUIUtility.singleLineHeight;
        //height += EditorGUIUtility.singleLineHeight;
        if (imageSettingFoldout)
            DrawParticipantImageSettings(property, ref rect);
        else height += EditorGUIUtility.singleLineHeight;

    }

    private void DrawDialogSettings(SerializedProperty _dialogSettings, ref Rect rect)
    {

        // base.OnGUI(position, property, label);
        EditorGUI.PropertyField(rect, _dialogSettings, new GUIContent(_dialogSettings.displayName), true);
        var propertyHeight = EditorGUI.GetPropertyHeight(_dialogSettings, true);
        height += propertyHeight + EditorGUIUtility.singleLineHeight; ;
        rect.y += propertyHeight;
    }

    private void DrawParticipantSelectors(SerializedProperty _participantSelectors,ref Rect rect)
    {

        // base.OnGUI(position, property, label);
        EditorGUI.PropertyField(rect, _participantSelectors, new GUIContent(_participantSelectors.displayName), true);
        var propertyHeight = EditorGUI.GetPropertyHeight(_participantSelectors, true);
        height += propertyHeight;
        rect.y += propertyHeight;
    }

    private void DrawParticipantImageSettings(SerializedProperty property, ref Rect rect)
    {
        lock (property)
        {

            var _participantsImageSettings = property.FindPropertyRelative("_participantsImageSettings");
            var _participantSelectors = property.FindPropertyRelative("_participantSelectors");
            var _participantSelectorsInfoList = _participantSelectors.FindPropertyRelative("_participantNames");

            var _participantsImageSettings_namesList =
                _participantsImageSettings.FindPropertyRelative("_participantNamesList");
            _participantsImageSettings_namesList.ClearArray();
            for (int i = 0; i < _participantSelectorsInfoList.arraySize; i++)
            {
                _participantsImageSettings_namesList.InsertArrayElementAtIndex(_participantsImageSettings_namesList.arraySize);
                var cell = _participantsImageSettings_namesList.GetArrayElementAtIndex(_participantsImageSettings_namesList.arraySize-1);
                cell.stringValue =
                    _participantSelectorsInfoList.GetArrayElementAtIndex(i).FindPropertyRelative("participantName")
                        .stringValue;
            }



            EditorGUI.PropertyField(rect, _participantsImageSettings,
                new GUIContent(_participantsImageSettings.displayName), true);
            var propertyHeight = EditorGUI.GetPropertyHeight(_participantsImageSettings, true);
            height += propertyHeight;
            rect.y += propertyHeight;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return  height;
    }
    

}



public enum ParticipantImagePosition
{
    Auto,
    Left,
    Right,
}

[Serializable]
public class ParticipantImageSettingsData
{
    [SerializeField] private string _participantName;
    [SerializeField] private ParticipantImagePosition _imagePosition;
    [SerializeField] private int _participantSelectedSpriteIndex;

    public string ParticipantName
    {
        get { return _participantName; }
        set { _participantName = value; }
    }

    public ParticipantImagePosition Position
    {
        get { return _imagePosition; }
        set { _imagePosition = value; }
    }

    public int ParticipantSelectedSpriteIndex
    {
        get { return _participantSelectedSpriteIndex; }
        set { _participantSelectedSpriteIndex = value; }
    }
}


[Serializable]
public class ParticipantImageSettingsList : IList<ParticipantImageSettingsData>
{
    [SerializeField] private List<ParticipantImageSettingsData> _data = new List<ParticipantImageSettingsData>();
    [SerializeField] List<string> _participantNamesList;



    public IEnumerator<ParticipantImageSettingsData> GetEnumerator()
    {
        return _data.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable) _data).GetEnumerator();
    }
    public void Add(ParticipantImageSettingsData item)
    {
        _data.Add(item);
    }
    public void Clear()
    {
        _data.Clear();
    }
    public bool Contains(ParticipantImageSettingsData item)
    {
        return _data.Contains(item);
    }
    public void CopyTo(ParticipantImageSettingsData[] array, int arrayIndex)
    {
        _data.CopyTo(array, arrayIndex);
    }
    public bool Remove(ParticipantImageSettingsData item)
    {
        return _data.Remove(item);
    }
    public int Count
    {
        get { return _data.Count; }
    }
    public bool IsReadOnly
    {
        get { return false; }
    }
    public int IndexOf(ParticipantImageSettingsData item)
    {
        return _data.IndexOf(item);
    }
    public void Insert(int index, ParticipantImageSettingsData item)
    {
        _data.Insert(index, item);
    }
    public void RemoveAt(int index)
    {
        _data.RemoveAt(index);
    }
    public ParticipantImageSettingsData this[int index]
    {
        get { return _data[index]; }
        set { _data[index] = value; }
    }
}



[CustomPropertyDrawer(typeof(ParticipantImageSettingsList))]
public class ParticipantImageSettingsListDrawer : PropertyDrawer
{

    private ReorderableList _reorderableList;
    private List<ParticipantImageSettingsData> _dataClone;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        var _participantNamesList = property.FindPropertyRelative("_participantNamesList");


        InitlializeDataCloneataClone(_participantNamesList);

        if (_reorderableList == null)
            UpdateDataListValues(property);//first time initialization

        if (AddDataCloneNewItems(_participantNamesList))
            UpdateDataListValues(property);

        if(RemoveDataCloneExtraItems(_participantNamesList))
         UpdateDataListValues(property);


        InitializeReorderableList(property, label);

        _reorderableList.DoList(position);


    }

    private void UpdateDataListValues(SerializedProperty property)
    {
        lock (property)
        {
               var _dataList = property.FindPropertyRelative("_data");

        _dataList.ClearArray();
        for (int i = 0; i < _dataClone.Count; i++)
        {
            _dataList.InsertArrayElementAtIndex(_dataList.arraySize);
            var arrayElementAtIndex = _dataList.GetArrayElementAtIndex(_dataList.arraySize - 1);

            arrayElementAtIndex.FindPropertyRelative("_participantName").stringValue = _dataClone[i].ParticipantName;
            arrayElementAtIndex.FindPropertyRelative("_imagePosition").enumValueIndex = (int)_dataClone[i].Position;
            arrayElementAtIndex.FindPropertyRelative("_participantSelectedSpriteIndex").intValue =
                _dataClone[i].ParticipantSelectedSpriteIndex;
        }
        }
     
    }

    private void InitlializeDataCloneataClone(SerializedProperty _participantNamesList)
    {
        if (_dataClone == null)
        {
            _dataClone = new List<ParticipantImageSettingsData>();

            for (int i = 0; i < _participantNamesList.arraySize; i++)
            {
                var itemParticipantName = _participantNamesList.GetArrayElementAtIndex(i).stringValue;

                var temp = new ParticipantImageSettingsData();
                temp.ParticipantName = itemParticipantName;
                temp.ParticipantSelectedSpriteIndex = DialogManager.GetInstance().GetAllParticipants().FirstOrDefault(x => x.Name.Equals(itemParticipantName)).DefaultSpriteIndex;
                temp.Position = ParticipantImagePosition.Auto;
                _dataClone.Add(temp);
            }


        }
    }

    bool  AddDataCloneNewItems(SerializedProperty _participantNamesList)
    {
        lock (_participantNamesList)
        {
            bool added = false;
            for (int i = 0; i < _participantNamesList.arraySize; i++)
            {

                var itemParticipantName = _participantNamesList.GetArrayElementAtIndex(i).stringValue;
                if (_dataClone.FirstOrDefault(x => x.ParticipantName.Equals(itemParticipantName)) == null)
                {
                    var dialogParticipant = DialogManager.GetInstance().GetAllParticipants()
                        .FirstOrDefault(x => x.Name.Equals(itemParticipantName));
                    if (dialogParticipant != null)
                    {
                        var temp = new ParticipantImageSettingsData();
                        temp.ParticipantName = itemParticipantName;


                        temp.ParticipantSelectedSpriteIndex = dialogParticipant.DefaultSpriteIndex;
                        temp.Position = ParticipantImagePosition.Auto;
                        _dataClone.Add(temp);
                        added = true;
                    }
                }

            }
            return added;
        }
    }

    bool  RemoveDataCloneExtraItems(SerializedProperty _participantNamesList)
    {
        lock (_participantNamesList)
        {
            bool removed = false;
            for (int i = 0; i < _dataClone.Count; i++)
            {
                bool found = false;
                for (int j = 0; j < _participantNamesList.arraySize && !found; j++)
                {
                    var itemParticipantName = _participantNamesList.GetArrayElementAtIndex(j).stringValue;
                    if (itemParticipantName.Equals(_dataClone[i].ParticipantName))
                        found = true;
                }
                if (!found)
                {
                    removed = true;
                    _dataClone.RemoveAt(i);
                    i--;
                }
            }
            return removed;
        }
    }

    private void InitializeReorderableList(SerializedProperty property, GUIContent label)
    {

        //InitializeReorderableList
        if (_reorderableList == null)
        {

            var _participantNamesList = property.FindPropertyRelative("_participantNamesList");

            _reorderableList = new ReorderableList(property.serializedObject, _participantNamesList, false, true, false, false);
            _reorderableList.elementHeight = EditorGUIUtility.singleLineHeight * 3;

            _reorderableList.drawElementCallback += delegate (Rect rect, int index2, bool active, bool focused)
            {
                var itemParticipantName = _participantNamesList.GetArrayElementAtIndex(index2).stringValue;


                // var dataItem = _dataList.GetArrayElementAtIndex(index2);
                // var participantPosEnum = dataItem.FindPropertyRelative("_imagePosition");
                var dataCloneItem =
                    _dataClone.FirstOrDefault(x => x.ParticipantName.Equals(itemParticipantName));
                if (dataCloneItem == null) return;


                Rect pos = rect;
                pos.height = EditorGUIUtility.singleLineHeight;
                pos.width = rect.width / 3;
                EditorGUI.LabelField(pos, new GUIContent(itemParticipantName, itemParticipantName));


                //pos.y += pos.height;
                pos.x += pos.width;
                pos.width = rect.width * 2 / 3;
                dataCloneItem.Position = (ParticipantImagePosition)EditorGUI.EnumPopup(pos, "Image Pos", dataCloneItem.Position);

                pos.y += pos.height;
                dataCloneItem.ParticipantSelectedSpriteIndex = EditorGUI.Popup(pos, "Image Sprite",
                    dataCloneItem.ParticipantSelectedSpriteIndex,
                   ParticipantsUtility.GetParticipantSpriteNames(DialogManager.GetInstance().GetAllParticipants().Find(x => x.Name.Equals(itemParticipantName))).ToArray());

                pos.y += pos.height;
                pos.width = rect.width;
                pos.x = rect.x;
                EditorGUI.LabelField(pos, new GUIContent("-----------------------------------------------------------------------------------------------------"));

                // pos.y += pos.height;
            };
            _reorderableList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, label);
            };
            
        }
    }




    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (_reorderableList != null)
            return base.GetPropertyHeight(property, label) + _reorderableList.GetHeight();
        return base.GetPropertyHeight(property, label);
    }
    
    


}












public class ParticipantSelectorAttribute : PropertyAttribute
{
//todo to implement class
    public ParticipantSelectorAttribute()
    {
    }
}