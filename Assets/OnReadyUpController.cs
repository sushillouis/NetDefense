using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnReadyUpController : MonoBehaviour {
    public Text startWaveText;
    public Text readyUpPlayersValueText;

    public void OnReadyUp() {
        Shared.inst.setPlayerToReady(SharedPlayer.playerIdForThisDevice);
        AutoHelpScreenBlackhatManager.inst.OnReady();

        gameObject.GetComponentInParent<ButtonSelectionEffect>().OnTurnOff();
    }

    public void Update() {
        startWaveText.text = "Start Wave " + (Game_Manager.inst.currentWave + 1);
        readyUpPlayersValueText.text = "Players Ready: " + Shared.inst.getPlayersReady() + "/" + Shared.inst.players.Count;
    }
}
