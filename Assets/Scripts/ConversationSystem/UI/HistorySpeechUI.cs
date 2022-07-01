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

    public void SetValues(Speech speech)
    {
        if(speech == null) return;
        this.personName.SetText(speech.speaker.firstName);
        this.actionTitle.SetText(speech.action.title);
        this.speechType.SetText(speech.action.type.name);
        this.topic.SetText(speech.action.topic?.name ?? "[]");
    }
}
