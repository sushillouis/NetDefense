using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ServerElementManager : MonoBehaviour {
    public Text gameNameText;
    public string gameName;
    public string ipAddress;


    public void OnDataChanged() {
        gameNameText.text = gameName;
    }

    public void OnJoinGamePressed() {
        NetworkManager.singleton.networkAddress = ipAddress;
        NetworkManager.singleton.StartClient();
        LobbyMenuManager.inst.CurrentState = LobbyStates.READY_UP_STATE;

    }
}
