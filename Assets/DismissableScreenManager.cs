using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DismissableScreenManager : MonoBehaviour {

    public Button[] tabs;
    public Displaceable[] contents;
    public Displaceable mainContent;
    public int activeIndex;

    public static DismissableScreenManager inst;

    private void Awake() {
        inst = this;
    }


    void Start() {
        OnTabPressed(0);
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.Space) && (Shared.inst.gameState.currentState == SharedGameStates.PLAY || Shared.inst.gameState.currentState == SharedGameStates.OVER)) {
            mainContent.isValid = !mainContent.isValid;
            
        }

        if (activeIndex == 2) {
            ScoreManager.inst.OnMetricsUpdated();
            RealtimeOverviewManager.inst.OnRefreshMenu();
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
        this.activeIndex = activeIndex;

        BarchartManager.getBarManagerByName("Chart1").shouldUpdate = activeIndex == 0;
        BarchartManager.getBarManagerByName("Chart2").shouldUpdate = activeIndex == 1;


    }
}
