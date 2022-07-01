using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConversationHistory
{
    [SerializeField]
    private List<Speech> history;

    public ConversationHistory()
    {
        history = new List<Speech>();
    }

    public void Add(Speech speech)
    {
        history.Add(speech);
    }

    public Speech Last()
    {
        if (history.Count == 0) return null;
        return history[history.Count - 1];
    }
}
