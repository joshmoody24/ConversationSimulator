using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Speech
{
    public SpeechAction action;
    public Topic topic;
    public Person speaker;
    public Person receiver;
    public UnityAction onResolve;
    public string data;
    
    public void OnResolve(UnityAction action)
    {
        onResolve += action;
    }

    public void Resolve()
    {
        onResolve.Invoke();
    }

    public Speech(SpeechAction action, Person speaker, Person receiver, Topic topic, string data = "")
    {
        this.action = action;
        this.speaker = speaker;
        this.receiver = receiver;
        this.topic = topic;
        this.data = data;
    }
}
