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
            player.knowledge.Add(new Knowledge { topic = t, amount = 0.5f });
            npc.knowledge.Add(new Knowledge { topic = t, amount = 0.5f });
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
        List<SpeechAction> actions = conversation.PossibleActions(player);
        ui.StartTurn(player);
    }

    public void EndPlayerTurn(SpeechAction action)
    {
        AddToConversation(action, activePerson, new List<Person>() { npc });
        ui.playerControls.SetActive(false);
        activePerson = npc;
        StartCoroutine(npcTurn());
    }

    public void AddToConversation(SpeechAction action, Person person, List<Person> receivers, Topic topic = null)
    {
        Speech newSpeech = new Speech(action, person, receivers, topic);
        conversation.AddToConversation(newSpeech);
        ui.AddToConversation(newSpeech);
        ui.UpdateStack(conversation.resolutionStack.Peek());
    }

    IEnumerator npcTurn()
    {
        yield return new WaitForSeconds(npcDelay);
        List<SpeechAction> possibleActions = conversation.PossibleActions(npc);
        if (possibleActions.Count != 0)
        {
            int randomIndex = Random.Range(0, possibleActions.Count);
            SpeechAction action = possibleActions[randomIndex];
            AddToConversation(action, npc, new List<Person>() { player });
        }
        activePerson = player;
        BeginPlayerTurn();
    }

}
