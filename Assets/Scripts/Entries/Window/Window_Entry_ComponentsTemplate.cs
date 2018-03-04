using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Window_Entry_ComponentsTemplate : Window_EntryBase
{

    [NonSerialized] private bool initialized;
    private Window_Entry_Components _componentsWindow;


    public override void OnGUI()
    {
        if (EntryData == null || _componentsWindow == null)
        {
            EditorGUILayout.LabelField(
                "Please initialize the window by calling the \"Initialize\" method in order to view the content.");
            return;
        }
        if (!initialized)
        {
            initialized = true;
            _componentsWindow.ComponentsReorderableList.Callback_Draw_Element = CallbackDrawElement;
        }
        if (_componentsWindow != null)
            _componentsWindow.OnGUI();

    }




    public override void Initialize(EntryBase data)
    {
        base.Initialize(data);
        var temp = (data as Entry_ComponentsEntryTemplate);
        _componentsWindow = (Window_Entry_Components)temp.TemplateInstance.GetNewWindow();
        foreach (var templateInstanceComponet in temp.TemplateInstance.Componets)
        {
            templateInstanceComponet.OnEditModeModified += Repaint;
            templateInstanceComponet.OnViewModeModified += Repaint;

        }
        initialized = false;
    }
    private void CallbackDrawElement(IList<EntryComponent> list, Rect rect, int index, bool isActive, bool isFocused)
    {
        var entryData = (Entry_ComponentsEntryTemplate)EntryData;
        if (!entryData.TemplateInstance.ShowEditMode)
        {
            float buttonWidth = 45;

            var pos = rect;
            pos.width -= buttonWidth;
            _componentsWindow.ItemDrawer(list, pos, index, isActive, isFocused);

            pos = rect;
            pos.width = buttonWidth;
            pos.height = EditorGUIUtility.singleLineHeight;
            pos.x = rect.width - pos.width/2 -2;
            pos.y = rect.y + _componentsWindow.OnGetItemHeight(_componentsWindow.EntryData.Componets, index) / 2 - pos.height / 2;

            //on remove button pressed
            if (GUI.Button(pos, new GUIContent("Apply")))
                entryData.ApplyComponentToObservers(index);

        }
        else _componentsWindow.ItemDrawer(list, rect, index, isActive, isFocused);
    }


}