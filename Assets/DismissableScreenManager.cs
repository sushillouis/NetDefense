using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DismissableScreenManager : MonoBehaviour {

    public Button[] tabs;
    public Displaceable[] contents;
    public Displaceable mainContent;

    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.Space) && Shared.inst.gameState.currentState == SharedGameStates.PLAY) {
            mainContent.isValid = !mainContent.isValid;
        }

    }

    public void DisableContents() {
        foreach (Displaceable d in contents) {
            d.isValid = false;
        }
    }

    public void OnTabPressed(int activeIndex) {
        DisableContents();
        contents[activeIndex].isValid = true;
    }
}
