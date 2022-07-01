using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ConversationManager : MonoBehaviour
{

    public Person player;
    public Person npc;

    private Person activePerson;

    public Conversation conversation;

    public float npcDelay = 1f;

    // UI stuff
    public GameObject playerControls;
    public GameObject actionButtonPrefab;
    public GameObject typeButtonPrefab;
    public Transform buttonParent;

    public Transform historyParent;
    public GameObject historySpeechPrefab;

    // greeting
    public SpeechType startingSpeechType;

    // Start is called before the first frame update
    void Start()
    {
        conversation = new Conversation(startingSpeechType);
        activePerson = player;
        BeginPlayerTurn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearButtons()
    {
        foreach (Transform child in buttonParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void SelectType(SpeechType type, List<SpeechAction> possibleActions)
    {
        ClearButtons();
        PopulateWithActionButtons(type, possibleActions);
    }

    public void PopulateWithTypeButtons(List<SpeechAction> possibleActions)
    {
        Debug.Log("Populating type buttons. There are " + possibleActions.Count + " buttons");
        HashSet<SpeechType> types = new HashSet<SpeechType>();
        foreach (SpeechAction a in possibleActions)
        {
            types.Add(a.type);
        }

        foreach (SpeechType t in types)
        {
            GameObject button = Instantiate(typeButtonPrefab, buttonParent);
            UnityAction unityAction = null;
            button.GetComponentInChildren<TextMeshProUGUI>().SetText(t.name);
            unityAction += () => SelectType(t, possibleActions);
            button.GetComponent<Button>().onClick.AddListener(unityAction);
        }
    }

    public void PopulateWithActionButtons(SpeechType selectedType, List<SpeechAction> possibleActions)
    {
        List<SpeechAction> actionsOfType = possibleActions.Where(a => a.type == selectedType).ToList();
        foreach (SpeechAction a in actionsOfType)
        {
            GameObject button = Instantiate(actionButtonPrefab, buttonParent);
            UnityAction unityAction = null;
            button.GetComponentInChildren<TextMeshProUGUI>().SetText(a.title);
            unityAction += () => EndPlayerTurn(a);
            button.GetComponent<Button>().onClick.AddListener(unityAction);
        }
    }

    public void BeginPlayerTurn()
    {
        Debug.Log("Beginning player turn");
        playerControls.SetActive(true);
        List<SpeechAction> actions = conversation.PossibleActions(player);
        PopulateWithTypeButtons(actions);
    }

    public void EndPlayerTurn(SpeechAction action)
    {
        Debug.Log("Ending player turn");
        AddToConversation(action, activePerson);
        playerControls.SetActive(false);
        activePerson = npc;
        ClearButtons();
        StartCoroutine(npcTurn());
    }

    public void AddToConversation(SpeechAction action, Person person, Topic topic = null)
    {
        conversation.AddToConversation(new Speech(action, person, topic));
        Debug.Log("Adding to history");
        GameObject speechUI = Instantiate(historySpeechPrefab, historyParent);
        var ui = speechUI.GetComponent<HistorySpeechUI>();
        ui.SetValues(person.firstName, action.title, action.type.name, "[]");
    }

    IEnumerator npcTurn()
    {
        Debug.Log("Starting npc turn");
        yield return new WaitForSeconds(npcDelay);
        List<SpeechAction> possibleActions = conversation.PossibleActions(npc);
        if (possibleActions.Count != 0)
        {
            int randomIndex = Random.Range(0, possibleActions.Count);
            SpeechAction action = possibleActions[randomIndex];
            AddToConversation(action, npc);
        }
        activePerson = player;
        BeginPlayerTurn();
    }

}
