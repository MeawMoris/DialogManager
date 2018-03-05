using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BetterGenericMenu
{
    public enum GenericMenuAddingType
    {
        Item,
        Item_WithUserData,
        DisabledItem,
        Separator
    }



    private readonly List<string> _menuOptionsList;
    private readonly List<bool> _isEnabledList;
    private readonly List<GenericMenu.MenuFunction> _onSelectList;
    private readonly List<GenericMenu.MenuFunction2> _onSelectWithUserDataList;
    private readonly List<object> _userdataList;
    private readonly List<GenericMenuAddingType> _types;

    public BetterGenericMenu()
    {
        _menuOptionsList = new List<string>();
        _isEnabledList = new List<bool>();
        _onSelectList = new List<GenericMenu.MenuFunction>();
        _onSelectWithUserDataList = new List<GenericMenu.MenuFunction2>();
        _userdataList = new List<object>();
        _types = new List<GenericMenuAddingType>();
    }






    public void AddItem(string context, bool on, GenericMenu.MenuFunction function)
    {
        _types.Add(GenericMenuAddingType.Item);
        _menuOptionsList.Add(context);
        _isEnabledList.Add(on);
        _onSelectList.Add(function);
        _userdataList.Add(null);
        _onSelectWithUserDataList.Add(null);
    }
    public void AddItem(string context, bool on, GenericMenu.MenuFunction2 function,object userData)
    {
        _types.Add(GenericMenuAddingType.Item_WithUserData);
        _menuOptionsList.Add(context);
        _isEnabledList.Add(on);
        _onSelectList.Add(null);
        _userdataList.Add(userData);
        _onSelectWithUserDataList.Add(function);
    }
    public void AddDisabledItem(string context)
    {
        _types.Add(GenericMenuAddingType.DisabledItem);
        _menuOptionsList.Add(context);
        _isEnabledList.Add(false);
        _onSelectList.Add(null);
        _userdataList.Add(null);
        _onSelectWithUserDataList.Add(null);
    }
    public void AddSeparator(string path)
    {
        _types.Add( GenericMenuAddingType.Separator);
        _menuOptionsList.Add(path);
        _isEnabledList.Add(false);
        _onSelectList.Add(null);
        _userdataList.Add(null);
        _onSelectWithUserDataList.Add(null);
    }
    public void Clear()
    {
        _types.Clear();
        _menuOptionsList.Clear();
        _isEnabledList.Clear();
        _onSelectList.Clear();
        _userdataList.Clear();
        _onSelectWithUserDataList.Clear();
    }
    public void RemoveAt(int index)
    {
        _types.RemoveAt(index);
        _menuOptionsList.RemoveAt(index);
        _isEnabledList.RemoveAt(index);
        _onSelectList.RemoveAt(index);
        _userdataList.RemoveAt(index);
        _onSelectWithUserDataList.RemoveAt(index);
    }
    public void Remove(string path)
    {
        var index = _menuOptionsList.FindIndex(x => x.Equals(path));
        if (index != -1)
            RemoveAt(index);
    }
    public int Count
    {
        get { return _menuOptionsList.Count; }
    }
    public GenericMenu GetGenericMenu()
    {
        GenericMenu g = new GenericMenu();
        for (int i = 0; i < _menuOptionsList.Count; i++)
        {
            switch (_types[i])
            {
                case GenericMenuAddingType.Item:
                    g.AddItem(new GUIContent(_menuOptionsList[i]), _isEnabledList[i], _onSelectList[i]);
                    break;
                case GenericMenuAddingType.Item_WithUserData:
                    g.AddItem(new GUIContent(_menuOptionsList[i]), _isEnabledList[i], _onSelectWithUserDataList[i], _userdataList[i]);
                    break;
                case GenericMenuAddingType.DisabledItem:
                    g.AddDisabledItem(new GUIContent(_menuOptionsList[i]));

                    break;
                case GenericMenuAddingType.Separator:
                    g.AddSeparator(_menuOptionsList[i]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        return g;
    }
    public void AddToGenericMenu(ref GenericMenu menu, string path="")
    {
        if(menu == null)
            menu = new GenericMenu();

        for (int i = 0; i < _menuOptionsList.Count; i++)
        {
            switch (_types[i])
            {
                case GenericMenuAddingType.Item:
                    menu.AddItem(new GUIContent(path+_menuOptionsList[i]), _isEnabledList[i], _onSelectList[i]);
                    break;
                case GenericMenuAddingType.Item_WithUserData:
                    menu.AddItem(new GUIContent(path+_menuOptionsList[i]), _isEnabledList[i], _onSelectWithUserDataList[i], _userdataList[i]);
                    break;
                case GenericMenuAddingType.DisabledItem:
                    menu.AddDisabledItem(new GUIContent(path+_menuOptionsList[i]));

                    break;
                case GenericMenuAddingType.Separator:
                    menu.AddSeparator(path+_menuOptionsList[i]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    public void ShowAsContext()
    {
        GetGenericMenu().ShowAsContext();
    }




    public List<string> MenuOptionsList
    {
        get { return (_menuOptionsList); }
    }

    public List<bool> IsEnabledList
    {
        get { return (_isEnabledList); }
    }

    public List<GenericMenu.MenuFunction> OnSelectList
    {
        get { return (_onSelectList); }
    }
    public List<GenericMenu.MenuFunction2> OnSelectWithUserDataList
    {
        get { return (_onSelectWithUserDataList); }
    }
    public List<object> UserdataList
    {
        get { return _userdataList; }
    }
    public List<GenericMenuAddingType> GenericMenuItemType
    {
        get { return _types; }
    }


}