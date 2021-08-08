using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackhatTutorialManager : MonoBehaviour
{
    public GameObject[] dialogues;

    public GameObject tutorialUI;
    public int activeIndex;

    public GameObject targetingButton;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject o in dialogues)
            o.GetComponent<Displaceable>().isValid = (false);

        dialogues[0].GetComponent<Displaceable>().isValid = (true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnNextDialog(int index) {
        activeIndex = index;
        OnAdvancedToNextPartOfTutorial();

        dialogues[index].GetComponent<Displaceable>().isValid = (false);
        if (index + 1 > dialogues.Length-1) {
            tutorialUI.SetActive(false);
            return;
        }

        dialogues[index+1].GetComponent<Displaceable>().isValid = (true);

    }

    public void OnLastDialog(int index) {
        activeIndex = index;

        if (index == 0)
            return;
        dialogues[index].GetComponent<Displaceable>().isValid = (false);
        dialogues[index - 1].GetComponent<Displaceable>().isValid = (true);
    }

    public void OnDismiss() {
        dialogues[activeIndex].GetComponent<Displaceable>().isValid = (false);
    }

    public void OnResumeTutorial() {
        dialogues[activeIndex].GetComponent<Displaceable>().isValid = (true);
    }

    public void OnAdvancedToNextPartOfTutorial() {
        // guide user to targeting button
        targetingButton.GetComponent<ButtonSelectionEffect>().isOn = activeIndex == 1;
    }
}
