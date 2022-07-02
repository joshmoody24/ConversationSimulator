using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeechType", menuName = "Conversation/Speech Type", order = 1)]
public class SpeechType : ScriptableObject
{
    // e.g. questions are closed by answers
    // this can be null
    public SpeechType closedBy;
    public bool closableBySelf;
    public bool closableByOther;
    public bool changesTopic = false;
    public bool overrideStack = false;
    public List<Transition> transitions;
}

[System.Serializable]
public class Transition
{
    public SpeechType to;
    public bool fromHistory = true;
    public bool fromStack = true;
}
