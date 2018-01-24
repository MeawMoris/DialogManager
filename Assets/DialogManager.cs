using System.Collections.Generic;

public class DialogManager
{
    private static DialogManager _dialogManager;
    public static DialogManager GetInstance()
    {
        if (_dialogManager == null)
            _dialogManager = new DialogManager();
        return _dialogManager;

    }


    private DialogParticipant _anonymousParticipant;

    private DialogManager()
    {
        _anonymousParticipant = new DialogParticipant();
        _anonymousParticipant.Name = "anonymous";
    }


    private DialogManager Initialize(ConversationViewer viewer)
    {


        return this;
    }

    public List<DialogParticipant> GetAllParticipants()
    {
        //todo to implement
        //todo return a list of all participant including the anonymous one
        return new List<DialogParticipant>(){ AnonymousParticipant 
            ,new DialogParticipant(){Name = "Alex"}
            ,new DialogParticipant(){Name = "meaw"}
            ,new DialogParticipant(){Name = "Yoav"}           
        };

       // return null;
    }
    public DialogParticipant AnonymousParticipant
    {
        get { return _anonymousParticipant; } 
    }




}