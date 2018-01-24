using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogBase : MonoBehaviour
{

    private void Start()
    {
        ConversationManager c = new ConversationManager();
        // c.StartDialog()
    }

    /*
     * Dialog types:
     * -monologue
     * -dialog between more than one person (probably 2)
     * 
     * 
     * -interaction with items dialog
     * -world items dialog(w/o interaction)
     * 
     * 
     * to do steps:
     * 1)make a WinForm that creates manages the dialog/monologue and exports to an xml
     * 2)create a library/classes that read the xml file and turn it into an object instance
     * 3)display the dialog/monologue in unity
     */




}


//----------------------------------------------------------------------------------
public class ConversationViewer
{

}
public class DockedBottom_ConversationViewer : ConversationViewer { }
public class Bubble_ConversationViewer : ConversationViewer { }
public class text_ConversationViewer : ConversationViewer { }
//----------------------------------------------------------------------------------

//dialogComponent
/*public class DialogComponent
{
    public TextAsset DialogXml;
   // public reor
    
}*/

//----------------------------------------------------------------------------------

//static singleton class
public class ConversationManager
{
    private static ConversationManager _cManager;
    public static ConversationManager GetInstance(ConversationViewer viewer)
    {
        if (_cManager == null)
            _cManager = new ConversationManager();
        return _cManager.Initialize(viewer);
    }


    public ConversationManager()
    {

    }
    public ConversationManager Initialize(ConversationViewer viewer)
    {


        return this;
    }



    /// <summary>
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
    public bool IsDialogRunnig { get; private set; }





}

//----------------------------------------------------------------------------------




public class ConversationViewerSettings
{
    public bool MultipleSimultaneousDialogs { get; set; }
    public bool ShowParticipantImages { get; set; }
    public bool ShowParticimantNames { get; set; }
    public bool AutoPlay { get; set; }//todo Calculate Delay time bassed on text in slide

}


