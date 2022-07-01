using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    public AbstractStat abstractStat;
    [SerializeField]
    private float value;

    public void setValue(float amount)
    {
        float inv = 1 / abstractStat.stepSize;
        value = Mathf.Round(Mathf.Clamp(amount, abstractStat.min, abstractStat.max) * inv) / inv;
    }

    public float getValue()
    {
        return value;
    }
}
