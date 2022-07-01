using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ConversationUI : MonoBehaviour
{
    public GameObject playerControls;
    public GameObject buttonPrefab;
    public Transform buttonParent;
    public Button backButton;
    public Transform historyParent;
    public GameObject historySpeechPrefab;
    public Transform stackParent;

    private ConversationManager manager;

    private SpeechType type;
    private TopicSubcategory subcategory;
    private Topic topic;
    private Person person;

    private UnityAction onBack = null;

    public void SetManager(ConversationManager manager)
    {
        if (this.manager == null) this.manager = manager;
        else Debug.LogWarning("Tried to set the UI manager after it was already set. This is not allowed.");
    }

    public void Start()
    {
        // initialize back button eventually
        RestartTurn();
        backButton.onClick.AddListener(onBack);
    }

    public void RestartTurn()
    {
        type = null;
        subcategory = null;
        topic = null;
    }

    public void StartTurn(Person person)
    {
        this.person = person;
        PopulateWithTypes();
    }

    public void EndTurn(SpeechAction action)
    {
        type = null;
        subcategory = null;
        topic = null;
        person = null;
        manager.EndPlayerTurn(action);
    }

    public void UpdateStack(Speech latest)
    {
        foreach (Transform child in stackParent)
        {
            Destroy(child.gameObject);
        }
        if (latest == null) return;
        GameObject stackMessage = Instantiate(historySpeechPrefab, stackParent);
        var speechUI = stackMessage.GetComponent<HistorySpeechUI>();
        speechUI.SetValues(latest);
    }

    public void ClearButtons()
    {
        foreach (Transform child in buttonParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void SelectType(SpeechType type)
    {
        ClearButtons();
        this.type = type;
        if (type.changesTopic)
        {
            PopulateWithSubcategories();
        }
        else
        {
            PopulateWithActions();
        }
    }

    public void SelectSubcategory(TopicSubcategory sc)
    {
        ClearButtons();
        subcategory = sc;
        PopulateWithTopics();
    }

    public void SelectTopic(Topic topic)
    {
        ClearButtons();
        this.topic = topic;
        PopulateWithActions();
    }

    public void AddToConversation(Speech speech)
    {
        GameObject speechUI = Instantiate(historySpeechPrefab, historyParent);
        var ui = speechUI.GetComponent<HistorySpeechUI>();
        ui.SetValues(speech);
    }

    public void PopulateWithTypes()
    {
        ClearButtons();
        backButton.interactable = false;
        onBack = null;

        List<SpeechType> allTypes = Conversation.FindAllSpeechActions()
            .Select(a => a.type)
            .Distinct()
            .ToList();

        foreach (SpeechType t in allTypes)
        {
            SpawnButton(t.name, () => SelectType(t), IsUsable(t));
        }
    }

    public void SpawnButton(string title, UnityAction action, bool interactable)
    {
        GameObject button = Instantiate(buttonPrefab, buttonParent);
        button.GetComponentInChildren<TextMeshProUGUI>().SetText(title);
        UnityAction unityAction = null;
        unityAction += action;
        Button b = button.GetComponent<Button>();
        b.onClick.AddListener(unityAction);
        b.interactable = interactable;
    }

    public void PopulateWithActions()
    {
        ClearButtons();

        backButton.interactable = true;
        onBack = topic == null ? () => PopulateWithTypes() : () => PopulateWithTopics();

        List<SpeechAction> possibleActions = manager.conversation.PossibleActions(person)
            .Where(a => a.type == type)
            .ToList();

        foreach (SpeechAction a in possibleActions)
        {
            SpawnButton(a.title, () => EndTurn(a), IsUsable(a, topic));
        }
    }

    public void PopulateWithSubcategories()
    {
        ClearButtons();

        backButton.interactable = true;
        onBack = () => PopulateWithTypes();

        List<Topic> allTopics = Conversation.FindAllTopics();
        List<TopicSubcategory> allSubcategories = allTopics
            .Select(t => t.subcategory)
            .Distinct()
            .ToList();

        foreach (TopicSubcategory sc in allSubcategories)
        {
            SpawnButton(sc.name, () => SelectSubcategory(sc), IsUsable(sc) ); // previsouly 'usable'
        }
    }

    public void PopulateWithTopics()
    {
        ClearButtons();

        backButton.interactable = true;
        onBack = () => PopulateWithSubcategories();

        List<Topic> topics = Conversation.FindAllTopics()
            .Where(t => t.subcategory == subcategory)
            .ToList();

        foreach (Topic t in topics)
        {
            SpawnButton(t.name, () => SelectTopic(t), IsUsable(t)); //passedKnowledgeTest.Count > 0
        }
    }

    private bool IsUsable(TopicSubcategory sc)
    {
        List<Topic> allTopics = Conversation.FindAllTopics();
        List<Topic> topicsInSC = allTopics.Where(t => t.subcategory == sc).ToList();
        bool usable = topicsInSC.Where(t => IsUsable(t)).Count() > 0;
        return usable;
    }

    private bool IsUsable(SpeechType type)
    {
        List<SpeechType> possibleTypes = manager.conversation.PossibleActions(person)
             .Select(a => a.type)
             .Distinct()
             .ToList();

        return possibleTypes.Contains(type);
    }

    private bool IsUsable(SpeechAction action, Topic topic)
    {
        bool usable = person.TestKnowledge(topic, action.knowledgeThreshold) || action.knowledgeThreshold == 0 || topic == null;
        if (type != null) Debug.Log(action.title + " " + topic?.name + " = " + usable );
        return usable;
    }

    private bool IsUsable(Topic topic)
    {
        bool usable = manager.conversation.PossibleActions(person)
            .Where(a => a.type == type || type == null)
            .Where(a => IsUsable(a, topic)).Count() > 0;
        Debug.Log(usable);
        return usable;
    }
}
