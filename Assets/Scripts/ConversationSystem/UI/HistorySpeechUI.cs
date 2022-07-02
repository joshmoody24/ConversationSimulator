using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistorySpeechUI : MonoBehaviour
{
    public Text personName;
    public Text actionTitle;
    public Text data;
    public Text topic;

    public void SetValues(Speech speech)
    {
        if(speech == null) return;
        this.personName.text = speech.speaker.firstName;
        this.actionTitle.text = speech.action.title;
        this.data.text = speech.data;
        this.topic.text = speech.topic?.name ?? "No Topic";//
    }
}
