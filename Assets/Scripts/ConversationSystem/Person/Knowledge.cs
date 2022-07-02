using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Knowledge
{
    public float amount;
    public Topic topic;

    public Knowledge(Topic topic, float amount)
    {
        this.amount = amount;
        this.topic = topic;
    }
}
