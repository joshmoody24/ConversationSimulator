using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Topic", menuName = "Conversation/Topic", order = 1)]
public class Topic : ScriptableObject
{
    public TopicCategory category;
    public List<Religion> believedBy;
    public List<TopicWeight> influencedBy;
}
