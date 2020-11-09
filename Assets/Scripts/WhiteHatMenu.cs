using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhiteHatMenu : MonoBehaviour
{


    public Text cash_value_text;
    public Text sell_router_text;

    public Text black_hat_status;
    public Text endgame_timer;

    public static WhiteHatMenu inst;

    public void Awake() {
        inst = this;
    }

    public void OnBlackHatStatusChanged() {
        black_hat_status.text = Shared.inst.gameMetrics.blackhat_score + "";
    }

    public void OnTimerChange(int seconds) {
        endgame_timer.text = seconds + "";
    }


    public void OnCashChanged() {

        cash_value_text.text = "$" + Shared.inst.gameMetrics.whitehat_cash;

        if (EntityManager.inst != null && EntityManager.inst.isMultiplayer) {
            Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.UPDATE_WHITE_HAT_MONEY, Shared.inst.gameMetrics.whitehat_cash + ""));
        }
    }

    public void OnGetHoneyPotSelected() {
        foreach(Destination d in Destination.destinations) {
            d.isReadyTobeHoneypot = true;
            d.updateSelection();
        }
    }
}
