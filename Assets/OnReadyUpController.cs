using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnReadyUpController : MonoBehaviour
{
    public Text readyUpPlayersValueText;

    public void OnReadyUp() {
        Shared.inst.setPlayerToReady(SharedPlayer.playerIdForThisDevice);

    }

    public void Update() {
        readyUpPlayersValueText.text = Shared.inst.getPlayersReady() + "/" + Shared.inst.players.Count; 
    }
}
