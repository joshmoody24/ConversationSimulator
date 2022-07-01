using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Speech
{
    public SpeechAction action;
    public Topic topic;
    public Person speaker;

    public Speech(SpeechAction action, Person speaker, Topic topic = null)
    {
        this.action = action;
        this.speaker = speaker;
        this.topic = topic;
    }
}
