using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhiteHatMenu : MonoBehaviour {


    public Text cash_value_text;
    public Text sell_router_text;

    public Text black_hat_status; // this is the string for routers placable
    public Text endgame_timer;
    public Text endgame_timer_lbl;

    public GameObject honeyPotButton;

    public static WhiteHatMenu inst;

    public GameObject hidePanel;

    public Button GetButton;

    public ButtonSelectionEffect startButton;

    public void Awake() {
        inst = this;
    }

    private void Start() {

        honeyPotButton.SetActive(MainMenu.difficulty == Difficulty.HARD);
        OnRouterPlaced();
    }

    public void OnRouterPlaced() {
        black_hat_status.text = Game_Manager.inst.routersPlaceable + "";
        hidePanel.SetActive(Game_Manager.inst.routersPlaceable < 1);
        GetButton.interactable = !hidePanel.activeSelf;
    }

    public void OnTimerChange(int seconds) {
        endgame_timer.text = (seconds == 0 ? "" : seconds + "");

        endgame_timer_lbl.text = seconds == 0 ? "Waiting..." : "Ends in";
    }


    public void OnCashChanged() {

        cash_value_text.text = "$" + Shared.inst.gameMetrics.whitehat_cash;

        if (EntityManager.inst != null && EntityManager.inst.isMultiplayer) {
            Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.UPDATE_WHITE_HAT_MONEY, Shared.inst.gameMetrics.whitehat_cash + ""));
        }
    }

    public void OnGetHoneyPotSelected() {
        foreach (Destination d in Destination.destinations) {
            d.isReadyTobeHoneypot = true;
            d.updateSelection();
        }
    }
}
