using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Conversation
{
    public List<Speech> history;

    private SpeechType startType;

    public Conversation(SpeechType startType)
    {
        history = new List<Speech>();
        this.startType = startType;
    }

    // returns the topic (which might have been tweaked)
    public void AddToConversation(Speech speech)
    {
        history.Add(speech);
    }

    private List<SpeechType> GrammaticallyCorrectSpeechTypes(Person person)
    {
        // get possible speech types that can follow either
        // 1) most recent speech in the history, or
        // 2) most recent speech in the resolution stack
        Speech latestInHistory = history.LastOrDefault();

        if (latestInHistory == null) {
            return new List<SpeechType>() { startType };
        };

        HashSet<SpeechType> possibleTypes = new HashSet<SpeechType>();
        foreach (Transition t in latestInHistory.action.type.transitions)
        {
            possibleTypes.Add(t.to);
        }

        return possibleTypes.ToList();
    }

    public Topic CurrentTopic()
    {
        return history.Last()?.topic;
    }

    public List<SpeechAction> GrammaticallyCorrectActions(Person person)
    {
        List<SpeechType> possibleTypes = GrammaticallyCorrectSpeechTypes(person);
        List<AbstractStat> personStats = person.stats.Select(s => s.abstractStat).ToList();

        List<SpeechAction> allActions = FindAllSpeechActions();

        // filter by type
        List<SpeechAction> possibleActions = allActions
            .Where(a => possibleTypes.Contains(a.type))
            .Where(a => person.HasPrerequisiteStats(a))
            .ToList();

        return possibleActions;
    }



    // UPDATE: might not be necessary since everything is spiderwebbed up
    // finds all scriptable objects by type
    public static List<SpeechType> FindAllSpeechTypes()
    {
        List<SpeechType> types = new List<SpeechType>();
        string[] assetNames = AssetDatabase.FindAssets("t:SpeechType");
        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            SpeechType type = AssetDatabase.LoadAssetAtPath<SpeechType>(SOpath);
            types.Add(type);
        }
        return types;
    }

    // finds all scriptable objects by type
    public static List<SpeechAction> FindAllSpeechActions()
    {
        List<SpeechAction> actions = new List<SpeechAction>();
        string[] assetNames = AssetDatabase.FindAssets("t:SpeechAction");
        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            SpeechAction action = AssetDatabase.LoadAssetAtPath<SpeechAction>(SOpath);
            actions.Add(action);
        }
        return actions;
    }

    public static List<TopicCategory> FindAllTopicCategories()
    {
        List<TopicCategory> scs = new List<TopicCategory>();
        string[] assetNames = AssetDatabase.FindAssets("t:TopicSubcategory");
        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            TopicCategory sc = AssetDatabase.LoadAssetAtPath<TopicCategory>(SOpath);
            scs.Add(sc);
        }
        return scs;
    }

    public static List<Topic> FindAllTopics()
    {
        List<Topic> topics = new List<Topic>();
        string[] assetNames = AssetDatabase.FindAssets("t:Topic");
        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            Topic t = AssetDatabase.LoadAssetAtPath<Topic>(SOpath);
            topics.Add(t);
        }
        return topics;
    }
}
