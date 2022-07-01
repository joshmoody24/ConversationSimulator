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
    public List<Knowledge> knowledge;
    public List<Commitment> commitments;

    public bool TestKnowledge(Topic topic, float threshold)
    {
        Knowledge k = knowledge.FirstOrDefault(k => k.topic == topic);
        if (k == null) return false;
        else return k.amount >= threshold;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
