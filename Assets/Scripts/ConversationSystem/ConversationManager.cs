using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ConversationManager : MonoBehaviour
{

    public Person player;
    public Person npc;

    private Person activePerson;

    public Conversation conversation;

    public float npcDelay = 1f;

    // greeting
    public SpeechType startingSpeechType;

    public ConversationUI ui;

    // Start is called before the first frame update
    void Start()
    {
        conversation = new Conversation(startingSpeechType);

        // automatically make a knowledge record for each player
        List<Topic> allTopics = Conversation.FindAllTopics();
        foreach(Topic t in allTopics)
        {
            player.knowledge.Add(new Knowledge { topic = t, amount = Random.Range(0f,1f) });
            npc.knowledge.Add(new Knowledge { topic = t, amount = Random.Range(0f,1f)});
        }

        ui.SetManager(this);
        activePerson = player;
        BeginPlayerTurn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeginPlayerTurn()
    {
        ui.playerControls.SetActive(true);
        List<SpeechAction> actions = conversation.GrammaticallyCorrectActions(player);
        ui.StartTurn(player);
    }

    public void EndPlayerTurn(SpeechAction action, Topic topic)
    {
        AddToConversation(action, activePerson, new List<Person>() { npc }, topic);
        ui.playerControls.SetActive(false);
        activePerson = npc;
        StartCoroutine(npcTurn());
    }

    public void AddToConversation(SpeechAction action, Person person, List<Person> receivers, Topic topic)
    {
        if (topic == null) topic = conversation.history.Last()?.topic;
        Speech newSpeech = new Speech(action, person, receivers, topic);
        conversation.AddToConversation(newSpeech);
        ui.AddToConversation(newSpeech);
        ui.UpdateStack(conversation.resolutionStack.Peek());
    }

    IEnumerator npcTurn()
    {
        yield return new WaitForSeconds(npcDelay);

        List<Topic> possibleTopics = npc.knowledge.Select(k => k.topic).Distinct().ToList();
        Topic selectedTopic = null;
        if(possibleTopics.Count != 0) selectedTopic = possibleTopics[Random.Range(0, possibleTopics.Count)];

        List<SpeechAction> possibleActions = conversation.GrammaticallyCorrectActions(npc);
        SpeechAction selectedAction;
        if (possibleActions.Count != 0)
        {
            int randomIndex = Random.Range(0, possibleActions.Count);
            selectedAction = possibleActions[randomIndex];
            Topic actualTopic = selectedAction.type.changesTopic ? selectedTopic : null;
            AddToConversation(selectedAction, npc, new List<Person>() { player }, actualTopic);
        }

        activePerson = player;
        BeginPlayerTurn();
    }

}
