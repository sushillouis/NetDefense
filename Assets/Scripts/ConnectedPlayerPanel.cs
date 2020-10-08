using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectedPlayerPanel : MonoBehaviour {

    public int playerID;

    public string username = "pending...";
    public string role = "pending...";

    public bool isReady;
    public bool isHost;

    public Text playerIDText;
    public Text usernameText;
    public Text roleText;
    public Text isReadyText;
    public Text isHostText;

    public void OnSettingsChanged() {
        playerIDText.text = "Player ID: " + playerID;
        usernameText.text = "Username: " + username;
        roleText.text = "Role: " + role;
        isReadyText.text = isReady ? "Ready" : "Not Ready";
        isHostText.text = isHost ? "Host" : "Not Host";
    }
}
