using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


[System.Serializable]
public class NetworkValues {

    public int port = 7777;
    public string ip = "47.36.68.105";


}

public class NetworkManagerHLAPI : NetworkManager {

    public NetworkValues networkValues;

    public static bool isMultiplayer = false;
    private static int net_id = 0;

    public static void reset() {
        isMultiplayer = false;
        net_id = 0;
    }

    public void LauchHost(bool mIsMultplayer) {
        singleton.networkPort = networkValues.port;
        isMultiplayer = mIsMultplayer;
        singleton.StartHost();
        try {
            NetworkDiscoveryManager.inst.StartAsServer();
        } catch {

        }
    }

    public void LauchClient() {
        singleton.networkAddress = networkValues.ip;
        singleton.networkPort = networkValues.port;

        singleton.StartClient();
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        bool isSinglePlayer = (Shared.inst.players.Count < 1 && !isMultiplayer);
        if ((Shared.inst.players.Count < 3 && isMultiplayer) || isSinglePlayer) {
            base.OnServerAddPlayer(conn, playerControllerId);

            Shared.inst.getOrAddPlayerById(net_id++);
            Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.ADD_NEW_PLAYER, Shared.inst.getIdString()));

            
        }
    }


}
