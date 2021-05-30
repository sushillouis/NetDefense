using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealtimeOverviewManager : MonoBehaviour
{
    public Text whitehatName;
    public Text blackhatName;

    public Text whitehatScoreValue;
    public Text blackhatScoreValue;

    public Text metric1;
    public Text metric2;
    public Text metric3;

    public Text titleText;

    public static RealtimeOverviewManager inst;

    private void Awake() {
        inst = this;
    }

    public void OnRefreshMenu() {

        titleText.text = Shared.inst.gameMetrics.whitehat_score > Shared.inst.gameMetrics.blackhat_score ? "Whitehat winning" : "Blackhat winning";

        if (!MainMenu.isMultiplayerSelectedFromMenu || Shared.inst.gameMetrics.metrics == null) {
            metric1.text = "Normal Spawned: " + ScoreManager.inst.totalFriendlyPacketsSpawned;
            metric2.text = "Infected Spawned: " + ScoreManager.inst.totalBadPacketsSpawned;
            metric3.text = "Total Spawned: " + ScoreManager.inst.totalPacketsSpawned;
        } else {
            metric1.text = "Normal Spawned: " + Shared.inst.gameMetrics.metrics[0];
            metric2.text = "Infected Spawned: " + Shared.inst.gameMetrics.metrics[1];
            metric3.text = "Total Spawned: " + Shared.inst.gameMetrics.metrics[2];
        }
        bool isMultiplayer = MainMenu.isMultiplayerSelectedFromMenu;

        if (Shared.inst.getDevicePlayer().role == SharedPlayer.WHITEHAT) {
            whitehatName.text = Shared.inst.getDevicePlayer().username;
            blackhatName.text = isMultiplayer ? (Shared.inst.getDevicePlayer().playerID == 0 ? Shared.inst.players[1].username : Shared.inst.players[0].username) : "AI";
        }

        if (Shared.inst.getDevicePlayer().role == SharedPlayer.BLACKHAT) {
            blackhatName.text = Shared.inst.getDevicePlayer().username;
            whitehatName.text = isMultiplayer ? (Shared.inst.getDevicePlayer().playerID == 1 ? Shared.inst.players[0].username : Shared.inst.players[1].username) : "AI";
        }

        whitehatScoreValue.text = Shared.inst.gameMetrics.whitehat_score.ToString();
        blackhatScoreValue.text = Shared.inst.gameMetrics.blackhat_score.ToString();

    }
}
