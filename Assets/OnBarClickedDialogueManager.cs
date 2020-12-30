using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnBarClickedDialogueManager : MonoBehaviour
{

    public Text titleText;
    public Text data1;
    public Text data2;

    public void OnShowDialogue(string titleText, string data1, string data2) {
        this.titleText.text = titleText;
        this.data1.text = data1;
        this.data2.text = data2;
        gameObject.SetActive(true);
    }
    
    public void OnDismissClicked() {
        gameObject.SetActive(false);
    }
}
