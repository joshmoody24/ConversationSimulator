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
    public TMP_Dropdown topicFilter;
    public Transform historyParent;
    public GameObject historySpeechPrefab;
    public Transform stackParent;
    public TextMeshProUGUI actionScreenTitle;

    private ConversationManager manager;

    private SpeechType type;
    private TopicSubcategory subcategory;
    private Topic topic;
    private Person person;

    public void SetManager(ConversationManager manager)
    {
        if (this.manager == null) this.manager = manager;
        else Debug.LogWarning("Tried to set the UI manager after it was already set. This is not allowed.");
    }

    public void Start()
    {
        RestartTurn();

        // set up category filter
        List<TopicSubcategory> subcategories = Conversation.FindAllTopicSubcategories();
        topicFilter.ClearOptions();
        List<string> options = new List<string>() { "All Categories" };
        options.AddRange(subcategories.Select(sc => sc.name).ToList());
        topicFilter.AddOptions(options);
        topicFilter.onValueChanged.AddListener((value) => {
            TopicSubcategory selected = subcategories.FirstOrDefault(sc => sc.name == topicFilter.options[value].text);
            FilterBySubcategory(selected);
        });
    }

    public void RestartTurn()
    {
        type = null;
        subcategory = null;
        topic = null;
        actionScreenTitle.SetText("");
    }

    public void StartTurn(Person person)
    {
        this.person = person;
        PopulateWithTypes();
    }

    public void EndTurn(SpeechAction action, Topic topic)
    {
        manager.EndPlayerTurn(action, topic);
        type = null;
        subcategory = null;
        this.topic = null;
        person = null;
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
            PopulateWithTopics();
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
        backButton.onClick.RemoveAllListeners();

        actionScreenTitle.SetText("What action?");

        topicFilter.gameObject.SetActive(false);

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
        actionScreenTitle.SetText("In what style?");
        ClearButtons();

        backButton.interactable = true;
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(topic == null ? () => PopulateWithTypes() : () => PopulateWithTopics());

        topicFilter.gameObject.SetActive(false);

        List<SpeechAction> possibleActions = manager.conversation.GrammaticallyCorrectActions(person)
            .Where(a => a.type == type)
            .ToList();

        foreach (SpeechAction a in possibleActions)
        {
            SpawnButton(a.title, () => EndTurn(a, topic), IsUsable(a, topic));
        }
    }

    public void PopulateWithTopics()
    {
        ClearButtons();

        actionScreenTitle.SetText("What about?");

        backButton.interactable = true;
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() => PopulateWithTypes());

        List<Topic> topics = Conversation.FindAllTopics()
            .Where(t => t.subcategory == subcategory || subcategory == null)
            .ToList();

        topicFilter.gameObject.SetActive(true);

        foreach (Topic t in topics)
        {
            SpawnButton(t.name, () => SelectTopic(t), IsUsable(t)); //passedKnowledgeTest.Count > 0
        }
    }

    public void FilterBySubcategory(TopicSubcategory subcategory)
    {
        this.subcategory = subcategory;
        PopulateWithTopics();
    }

    private bool IsUsable(SpeechType type)
    {
        List<SpeechType> possibleTypes = manager.conversation.GrammaticallyCorrectActions(person)
             .Select(a => a.type)
             .Distinct()
             .ToList();

        return possibleTypes.Contains(type);
    }

    private bool IsUsable(SpeechAction action, Topic topic)
    {
        if (topic == null) topic = manager.conversation.CurrentTopic();
        bool usable = person.TestKnowledge(topic, action.knowledgeThreshold) || action.knowledgeThreshold == 0 || topic == null;
        return usable;
    }

    private bool IsUsable(Topic topic)
    {
        bool usable = manager.conversation.GrammaticallyCorrectActions(person)
            .Where(a => a.type == type || type == null)
            .Where(a => IsUsable(a, topic)).Count() > 0;
        return usable;
    }
}
