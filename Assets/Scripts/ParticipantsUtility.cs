using System.Collections.Generic;
using System.Linq;

public static class ParticipantsUtility
{
    public static List<string> GetParticipantNames(IList<DialogParticipant> participants)
    {
        if (participants == null) return new List<string>();

        return participants.ToList().Select(x => x.Name).ToList();
    }
    public static List<string> GetParticipantSpriteNames(DialogParticipant participants)
    {
        if (participants == null || participants.Sprites == null || participants.Sprites.Count == 0)
            return new List<string>();

        return participants.Sprites.Select(x => x.name).ToList();
    }
}