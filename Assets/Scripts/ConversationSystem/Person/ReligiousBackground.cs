using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ReligiousBackground
{
    public Religion raisedReligion;
    public float raisedReligionIntensity;
    public Religion currentReligion;
    public float currentReligionIntensity;

    public List<BackgroundInfo> backgroundInfo;

    public void GenerateReligiousBackground()
    {
        // each builds on the previous
        RandomizeReligion();
        RandomizeBackgroundInfo();
    }

    void RandomizeReligion()
    {
        List<Religion> allR = Religion.AllReligions();
        raisedReligion = RandomReligionWeighted(allR);
        if(Random.Range(0f,1f) < raisedReligion.chanceOfLeaving)
        {
            currentReligion = RandomReligionWeighted(allR.Except(new List<Religion>() { raisedReligion }).ToList());
        }
        else
        {
            currentReligion = raisedReligion;
        }
    }

    Religion RandomReligionWeighted(List<Religion> religions)
    {
        int totalPeople = religions.Sum(r => r.numberOfFollowers);
        List<float> scaledProbabilities = religions.Select(r => (float)r.numberOfFollowers / (float)totalPeople).ToList();
        float remainingProbability = Random.Range(0f, 1f);
        int iteration = 0;
        while (remainingProbability > 0)
        {
            remainingProbability -= scaledProbabilities[iteration];
            if(remainingProbability > 0) iteration++;
        }
        return religions[iteration];
    }

    void RandomizeBackgroundInfo()
    {
        backgroundInfo = BackgroundInfo.All()
            .Where(
                bg => bg.prerequisiteReligion == raisedReligion ||
                bg.prerequisiteReligion == currentReligion || 
                bg.prerequisiteReligion == null
             )
            .Where(bg => Random.Range(0f, 1f) < bg.likelihood)
            .ToList();

        Debug.Log(backgroundInfo.Count);
    }

    public float GetRandomTopicKnowledge(Topic topic)
    {
        float knowledgeAmount = 0f;

        if (topic.believedBy.Contains(raisedReligion)) knowledgeAmount += raisedReligionIntensity / 2f;
        if (topic.believedBy.Contains(currentReligion)) knowledgeAmount += currentReligionIntensity / 2f;

        foreach (TopicWeight tWeight in topic.influencedBy)
        {
            knowledgeAmount += tWeight.weight * (backgroundInfo.Contains(tWeight.backgroundInfo) ? 1f : 0f);
        }

        return Mathf.Clamp(knowledgeAmount, 0f, Person.MAX_KNOWLEDGE);
    }
}