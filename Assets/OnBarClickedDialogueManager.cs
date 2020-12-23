﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnBarClickedDialogueManager : MonoBehaviour
{

    public Text titleText;
    public Text packetName;
    public Text frequency;

    public void OnShowDialogue(string titleText, string packetName, int frequency) {
        this.titleText.text = titleText;
        this.packetName.text = packetName;
        this.frequency.text = "Frequency " + frequency;
        gameObject.SetActive(true);
    }
    
    public void OnDismissClicked() {
        gameObject.SetActive(false);
    }
}