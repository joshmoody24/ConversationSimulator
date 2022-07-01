using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeechEffect", menuName = "Conversation/Speech Effect", order = 1)]
public class SpeechEffect : ScriptableObject
{
    public SpeechAction action;
    public AbstractStat abstractStat;
    public float magnitude;
    public MutateType mutateType;

    public enum MutateType { add, multiply }
}
