using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Conversation
{
    public ConversationHistory history;
    public ResolutionStack resolutionStack;

    private SpeechType startType;

    public Conversation(SpeechType startType)
    {
        history = new ConversationHistory();
        resolutionStack = new ResolutionStack();
        this.startType = startType;
    }

    public void AddToConversation(Speech speech)
    {
        // handle the resolution stack
        Speech stackSpeech = resolutionStack.Peek();
        if(stackSpeech != null)
        {
            if (stackSpeech.action.type.closedBy == speech.action.type)
            {
                if(stackSpeech.speaker == speech.speaker && stackSpeech.action.type.closableBySelf)
                {
                    resolutionStack.Pop();
                    speech.topic = CurrentTopic();
                }
                else if(stackSpeech.speaker != speech.speaker && stackSpeech.action.type.closableByOther)
                {
                    resolutionStack.Pop();
                    speech.topic = CurrentTopic();
                }
            }
        }


        if (speech.action.type.closedBy != null)
        {
            resolutionStack.Push(speech);
        }

        history.Add(speech);
    }

    private List<SpeechType> GrammaticallyCorrectSpeechTypes(Person person)
    {
        // get possible speech types that can follow either
        // 1) most recent speech in the history, or
        // 2) most recent speech in the resolution stack
        Speech latestInHistory = history.Last();
        Speech latestInStack = resolutionStack.Peek();

        if (latestInHistory == null) {
            return new List<SpeechType>() { startType };
        };

        HashSet<SpeechType> possibleTypes = new HashSet<SpeechType>();
        foreach (Transition t in latestInHistory.action.type.transitions)
        {
            if(t.fromHistory) possibleTypes.Add(t.to);
        }
        if(latestInStack != null && latestInHistory.action.type.overrideStack == false)
        {
            foreach (Transition t in latestInStack.action.type.transitions)
            {
                if(t.fromStack) possibleTypes.Add(t.to);
            }
        }

        // handle closeables
        if(latestInStack?.closeableBy.Contains(person) == false)
        {
            possibleTypes.Remove(latestInStack.action.type.closedBy);
        }


        return possibleTypes.ToList();
    }

    public Topic CurrentTopic()
    {
        return resolutionStack.Peek()?.topic;
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

    public static List<TopicSubcategory> FindAllTopicSubcategories()
    {
        List<TopicSubcategory> scs = new List<TopicSubcategory>();
        string[] assetNames = AssetDatabase.FindAssets("t:TopicSubcategory");
        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            TopicSubcategory sc = AssetDatabase.LoadAssetAtPath<TopicSubcategory>(SOpath);
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
