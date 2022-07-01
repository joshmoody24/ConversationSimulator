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
    public List<SpeechPrereq> prereqs;

    public bool IsValidFor(Person person)
    {
        foreach(SpeechPrereq p in prereqs)
        {
            if (p.Validate(person) == false) return false;
        }
        return true;
    }
}
