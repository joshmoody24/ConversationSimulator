using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "Conversation/Abstract Stat", order = 1)]
public class AbstractStat : ScriptableObject
{
    public float min = 0;
    public float max = 1;
    public float stepSize = 0.01f;
    public bool mutable = true;
}
