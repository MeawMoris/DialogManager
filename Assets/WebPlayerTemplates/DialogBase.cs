using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogBase : MonoBehaviour
{

    private void Start()
    {
        //ConversationManager c = new ConversationManager();
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

//----------------------------------------------------------------------------------




public class ConversationViewerSettings
{


}


