using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HistorySpeechUI : MonoBehaviour
{
    public TextMeshProUGUI personName;
    public TextMeshProUGUI actionTitle;
    public TextMeshProUGUI speechType;
    public TextMeshProUGUI topic;

    public void SetValues(string personName, string actionTitle, string speechType, string topic)
    {
        this.personName.SetText(personName);
        this.actionTitle.SetText(actionTitle);
        this.speechType.SetText(speechType);
        this.topic.SetText(topic);
    }
}
