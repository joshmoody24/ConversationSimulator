using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Action", menuName = "Conversation/Speech Action", order = 1)]
public class SpeechAction : ScriptableObject
{
    public SpeechType type;
    // if topic is null, it can be dynamically chosen
    public Topic topic;
    public string title;
    [TextArea(5, 10)]
    public string description;
    public float knowledgeThreshold = 0f;
    public List<SpeechPrereq> prereqs;
    public List<SpeechEffect> effects;

    public bool IsValidFor(Person person, Speech latestOnStack)
    {
        // check prereqs
        foreach(SpeechPrereq p in prereqs)
        {
            if (p.Validate(person) == false) return false;
        }

        // check if closable
        SpeechType stackType = latestOnStack?.action.type;
        if (stackType?.closedBy == type)
        {
            if (stackType.closableBySelf == false && latestOnStack.speaker == person) return false;
            if (stackType.closableByOther == false && latestOnStack.speaker != person) return false;
        }
        return true;
    }
}
