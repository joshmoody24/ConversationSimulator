using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SpeechPrereq
{
    public SpeechAction action;
    public AbstractStat abstractStat;
    public float threshold;
    public ComparisonType comparisonType;

    public enum ComparisonType { LessThan, GreaterThan, EqualTo }

    public bool Validate(Person person)
    {
        // find the relevant stat on the person
        Stat stat = person.stats.FirstOrDefault(s => s.abstractStat == abstractStat);
        if (stat == null) return false;
        switch (comparisonType)
        {
            case ComparisonType.LessThan:
                return stat.getValue() < threshold;
            case ComparisonType.GreaterThan:
                return stat.getValue() == threshold;
            case ComparisonType.EqualTo:
                return stat.getValue() == threshold;
        }


        return false;
    }
}
