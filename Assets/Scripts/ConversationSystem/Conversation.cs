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
        history.Add(speech);
        if(speech.action.type.closedBy != null)
        {
            resolutionStack.Push(speech);
        }
    }

    private List<SpeechType> PossibleSpeechTypes()
    {
        // get possible speech types that can follow either
        // 1) most recent speech in the history, or
        // 2) most recent speech in the resolution stack
        SpeechType latestInHistory = history.Last()?.action.type;
        SpeechType latestInStack = resolutionStack.Peek()?.action.type;

        if (latestInHistory == null) {
            Debug.Log("returning starting type");
            return new List<SpeechType>() { startType };
        };

        HashSet<SpeechType> possibleTypes = new HashSet<SpeechType>();
        foreach(Transition t in latestInHistory.transitions)
        {
            possibleTypes.Add(t.to);
        }
        if(latestInStack != null)
        {
            foreach (Transition t in latestInStack?.transitions)
            {
                possibleTypes.Add(t.to);
            }
        }

        return possibleTypes.ToList();
    }

    public List<SpeechAction> PossibleActions(Person person)
    {
        List<SpeechType> possibleTypes = PossibleSpeechTypes();
        Debug.Log("possible types: " + possibleTypes.Count);
        List<AbstractStat> personStats = person.stats.Select(s => s.abstractStat).ToList();

        List<SpeechAction> allActions = FindAllSpeechActions();
        Debug.Log("all actions: " + allActions.Count);

        // filter by type
        List<SpeechAction> possibleActions = allActions
            .Where(a => possibleTypes.Contains(a.type))
            .Where(a => a.IsValidFor(person))
            .ToList();

        Debug.Log("possible actions: " + possibleActions.Count);

        return possibleActions;
    }



    // UPDATE: might not be necessary since everything is spiderwebbed up
    // finds all scriptable objects by type
    public static List<SpeechType> FindAllSpeechTypes()
    {
        List<SpeechType> types = new List<SpeechType>();
        string[] assetNames = AssetDatabase.FindAssets("t:SpeechType");
        Debug.Log("Speech type asset strings: " + assetNames.Length);
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
        Debug.Log("SpeechAction asset names: " + assetNames.Length);
        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            SpeechAction action = AssetDatabase.LoadAssetAtPath<SpeechAction>(SOpath);
            actions.Add(action);
        }
        return actions;
    }
}
