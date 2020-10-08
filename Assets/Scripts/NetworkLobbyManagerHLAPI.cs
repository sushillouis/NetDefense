using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkLobbyManagerHLAPI : NetworkLobbyManager
{
    public GameObject player;

    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
        MainMenu.isMultiplayerSelectedFromMenu = true;
    }

    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId) {

        GameObject player = Instantiate(this.player);

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

        Debug.Log("Called Method to build player");

        if(EntityManager.inst.isServer) {
            Shared.inst.gameMetrics.isWhiteHat = true;
        } 

        return player;
    }
}
