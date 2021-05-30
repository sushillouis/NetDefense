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
    }

	public void OnWaveStart(){
		gameObject.SetActive(false);
	}

	public void OnWaveEnd() {
		gameObject.SetActive(true);
	}

    public void Update() {
        startWaveText.text = "Start Wave " + (Game_Manager.inst.currentWave + 1);
        readyUpPlayersValueText.text = (MainMenu.isMultiplayerSelectedFromMenu && Shared.inst.isPlayerReady(SharedPlayer.playerIdForThisDevice) ? "Players Ready: " + Shared.inst.getPlayersReady() + "/" + Shared.inst.players.Count : "Click Here to Start");
    }
}
