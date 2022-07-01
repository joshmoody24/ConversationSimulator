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
    public List<Person> receivers;
    public List<Person> closeableBy;
    public UnityAction onResolve;
    
    public void OnResolve(UnityAction action)
    {
        onResolve += action;
    }

    public void Resolve()
    {
        onResolve.Invoke();
    }

    public Speech(SpeechAction action, Person speaker, List<Person> receivers, Topic topic = null, List<Person> closeableBy = null)
    {
        this.action = action;
        this.speaker = speaker;
        this.receivers = receivers;
        this.topic = topic;
        if (closeableBy == null)
        {
            closeableBy = new List<Person>();
            if (action.type.closableByOther)
            {
                closeableBy.AddRange(receivers);
            }
            if(action.type.closableBySelf)
            {
                closeableBy.Add(speaker);
            }
        }
        this.closeableBy = closeableBy;
    }
}
