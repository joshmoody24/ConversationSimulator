using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeechType", menuName = "Conversation/Speech Type", order = 1)]
public class SpeechType : ScriptableObject
{
    public List<Transition> transitions;
}

[System.Serializable]
public class Transition
{
    public SpeechType to;
}
