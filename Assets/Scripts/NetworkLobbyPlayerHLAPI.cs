using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkLobbyPlayerHLAPI : NetworkLobbyPlayer
{
    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
    }
}
