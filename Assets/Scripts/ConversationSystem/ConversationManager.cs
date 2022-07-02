using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ConversationManager : MonoBehaviour
{

    public Person player;
    public Person npc;

    public Conversation conversation;

    public float npcDelay = 1f;

    // greeting
    public SpeechType startingSpeechType;

    public ConversationUI ui;

    // Start is called before the first frame update
    void Start()
    {
        conversation = new Conversation(startingSpeechType);
        ui.SetManager(this);
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
        AddToConversation(action, player, npc, topic);
        ui.playerControls.SetActive(false);
        StartCoroutine(npcTurn());
    }

    public void AddToConversation(SpeechAction action, Person person, Person receiver, Topic topic)
    {
        if (topic == null) topic = conversation.history.Last()?.topic;
        Speech newSpeech = new Speech(action, person, receiver, topic);
        conversation.AddToConversation(newSpeech);
        ui.AddToConversation(newSpeech);
    }

    IEnumerator npcTurn()
    {
        yield return new WaitForSeconds(npcDelay);

        List<Topic> possibleTopics = npc.knowledge.Select(k => k.topic).Distinct().ToList();
        Topic selectedTopic = possibleTopics[Random.Range(0, possibleTopics.Count)];

        List<SpeechAction> possibleActions = conversation.GrammaticallyCorrectActions(npc);
        SpeechAction selectedAction;
        if (possibleActions.Count != 0)
        {
            int randomIndex = Random.Range(0, possibleActions.Count);
            selectedAction = possibleActions[randomIndex];
            AddToConversation(selectedAction, npc, player, selectedTopic);
        }

        BeginPlayerTurn();
    }

}
