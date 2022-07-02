using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Person : MonoBehaviour
{

    public string firstName;
    public string lastName;
    public string address;
    public List<Stat> stats;
    public ReligiousBackground religiousBackground;

    public List<Knowledge> knowledge;
    public List<Commitment> commitments;

    public static float MAX_KNOWLEDGE = 5f;

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.tag == "Player") return;
        religiousBackground.GenerateReligiousBackground();
        GenerateKnowledge();
    }

    public void GenerateKnowledge()
    {
        knowledge = Conversation.FindAllTopics()
            .Select(t => new Knowledge(t, religiousBackground.GetRandomTopicKnowledge(t)))
            .ToList();
    }

    public bool TestKnowledge(Topic topic, float threshold)
    {
        Knowledge k = knowledge.FirstOrDefault(k => k.topic == topic);
        if (k == null) return false;
        else {
            return k.amount >= threshold;
        }
    }

    // tests if they at least HAVE the prereq, not if they fullfill it
    public bool HasPrerequisiteStats(SpeechAction action)
    {
        foreach(SpeechPrereq p in action.prereqs)
        {
            if (!stats.Select(s => s.abstractStat).ToList().Contains(p.abstractStat)) return false;
        }
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
