using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Transition", menuName = "Conversation/Speech Transition", order = 1)]
public class SpeechTransition : ScriptableObject
{
    public SpeechType from;
    public SpeechType to;
    public float weight = 1f;
}
