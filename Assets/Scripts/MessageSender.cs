using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using System;
using UnityEngine.SceneManagement;


// should used enum smh fp
public static class MessageTypes {

    /**
     * MessageTypes are used to
     * inform clients how to process
     * a new event. They are use to
     * classify the rpc data as a header.
     * For each type will exist a unique
     * implementation.
     *
     */

    public const int IS_HOST_WHITE_HAT = 1;                 // host only
    public const int UPDATE_MALIC_PACKETS = 2;              // host to client
    public const int UPDATE_WHITE_HAT_MONEY = 3;            // host to client
    public const int SET_HONEY_POT_ACTIVATION = 5;          // host to client and vica versa
    public const int SET_SERVER_TARGETTING_PROBABILITY = 6; // host to client and vica versa
    public const int SET_SCORES = 7;                        // host to client
    public const int SET_NETWORK_STATE = 8;                 // host to client
    public const int ADD_NEW_PLAYER = 9;                    // host to client
    public const int SET_LOBBY_PLAYER_SETTINGS = 10;        // host to client and vica versa
    public const int SET_SCORE_DERRIVATIVES = 11;           // host to client
    public const int CHANGE_PACKET_LIFECYCLE_STATUS = 12;   // only clients may send
    public const int SET_METRICS = 13;   // sync metrics for end game
    public const int FORCE_GAME_SHUTDOWN = 14;   // closes all networking resources
    public const int SET_PLAYERS_READY_STATUS = 15;   // set networkplayer.isReady

}

public class MessageSender : NetworkBehaviour {

    public void RequestMessageToServer(int type, string data) {
        if (isLocalPlayer)
            CmdMessageToServer(type, data);
    }

    public void Update() {


        // Only update ONE of Unet's "player prefabs"
        if (!isLocalPlayer)
            return;

        // if we are the server
        if (isLocalPlayer && isServer) {
            if (Input.GetKeyDown(KeyCode.F1)) {
                Shared.inst.gameMetrics.isWhiteHat = !Shared.inst.gameMetrics.isWhiteHat;

                RequestMessageToServer(MessageTypes.IS_HOST_WHITE_HAT, Shared.inst.gameMetrics.isWhiteHat.ToString());
            }
        }


        foreach (SyncEvent se in Shared.inst.syncEvents) {
            RequestMessageToServer(se.id, se.data);
        }

        Shared.inst.syncEvents.Clear();

    }

    [Command]
    public void CmdMessageToServer(int type, string data) {
        RpcMessageFromServer(type, data);
    }

    [ClientRpc]
    public void RpcMessageFromServer(int type, string data) {

        if(type == MessageTypes.FORCE_GAME_SHUTDOWN) {
            Debug.Log("Trying to quit...");
            try {
                NetworkManager.singleton.StopClient();
                NetworkManager.singleton.StopHost();
                NetworkManager.singleton.StopServer();
                NetworkManager.singleton.StopAllCoroutines();
                Destroy(NetworkManager.singleton);
            } catch {
                Debug.LogError("Couldn't stop client or host");
            }

            SceneManager.LoadScene("MainMenu");
        }

        if(type == MessageTypes.SET_METRICS) {
            string[] raw_metrics = data.Split(',');
            Shared.inst.gameMetrics.metrics = new float[raw_metrics.Length];
            for(int i = 0; i < raw_metrics.Length; i++) {
                Shared.inst.gameMetrics.metrics[i] = float.Parse(raw_metrics[i]);
            }
            GameOverMenuManager.inst.OnRefreshMenu();
        }

        if (type == MessageTypes.SET_LOBBY_PLAYER_SETTINGS) {
            setPlayerSettings(data);
        }

        if(type == MessageTypes.SET_PLAYERS_READY_STATUS) {
            setPlayerIsReady(data);
        }

        // add new player
        if (type == MessageTypes.ADD_NEW_PLAYER) {
            addNewPlayers(data);
        }

        // update malic packet values
        if (type == MessageTypes.UPDATE_MALIC_PACKETS) {
            updateMalicPackets(data);
        }

        //update network state
        if (type == MessageTypes.SET_NETWORK_STATE) {
            setNetworkState(data);
        }

        // update honeypots as they become activated
        if (type == MessageTypes.SET_HONEY_POT_ACTIVATION) {
            setHoneyPotActivation(data);
        }

        // set server spawning probabilities
        if (type == MessageTypes.SET_SERVER_TARGETTING_PROBABILITY) {
            setTargettingProbabilities(data);
        }

        // change packet lifecycle status
        if (type == MessageTypes.CHANGE_PACKET_LIFECYCLE_STATUS) {
            changePacketLifecycleStatus(data);
        }

        // set scores
        if (type == MessageTypes.SET_SCORES) {
            string[] info = data.Split(',');
            int a = int.Parse(info[0]);
            int b = int.Parse(info[1]);

            Shared.inst.gameMetrics.blackhat_score = a;
            Shared.inst.gameMetrics.whitehat_score = b;

            // if we are a client
            if (!isLocalPlayer && !isServer)
                ScoreManager.inst.UpdateUIClient();
        }

        if (type == MessageTypes.SET_SCORE_DERRIVATIVES) {
            string[] info = data.Split(',');
            float a = float.Parse(info[0]);
            float b = float.Parse(info[1]);

            Shared.inst.gameMetrics.derrivative_blackhat_score = a;
            Shared.inst.gameMetrics.derrivative_whitehat_score = b;

            // if we are a client
            if (!isLocalPlayer && !isServer)
                ScoreManager.inst.UpdateUIClient();
        }

        // if we are a server or a server-client
        if (isLocalPlayer && isServer) {

            // update and sync ui
            if (type == MessageTypes.IS_HOST_WHITE_HAT) {
                Shared.inst.gameMetrics.isWhiteHat = Convert.ToBoolean(data);
            }

            // update money values
            if (type == MessageTypes.UPDATE_WHITE_HAT_MONEY) {
                Shared.inst.gameMetrics.whitehat_cash = Convert.ToInt32(data);
            }

        }

        // if we are a client recieving a message from the server
        if (!isLocalPlayer && !isServer) {

            // update and sync ui
            if (type == MessageTypes.IS_HOST_WHITE_HAT) {
                Shared.inst.gameMetrics.isWhiteHat = !Convert.ToBoolean(data);
                Debug.Log("Updated");

            }

            // update money values and the client ui
            if (type == MessageTypes.UPDATE_WHITE_HAT_MONEY) {
                Shared.inst.gameMetrics.whitehat_cash = Convert.ToInt32(data);
                WhiteHatMenu.inst.OnCashChanged();
            }
        }
    }

    public void setTargettingProbabilities(string data) {

        Dictionary<string, float> tp = Shared.inst.gameMetrics.target_probabilities;

        string[] hashmap = data.Split(',');

        string key = hashmap[0];
        float value = float.Parse(hashmap[1]);

        if (tp.ContainsKey(key))
            tp[key] = value;
        else
            tp.Add(key, value);


        PacketPoolManager.inst.OnBlackhatUpdateStrategy();

    }


    private void setHoneyPotActivation(string data) {

        string[] info = data.Split(',');

        string activatedID = (info[0]);
        bool value = bool.Parse(info[1]);
        Destination.getDestinationByID(activatedID).setIsHoneyPot(value);
    }

    private void updateMalicPackets(string data) {
        string[] vec3 = data.Split(',');

        Shared.inst.maliciousPacketProperties.shape = Convert.ToInt32(vec3[0]);
        Shared.inst.maliciousPacketProperties.size = Convert.ToInt32(vec3[1]);
        Shared.inst.maliciousPacketProperties.color = Convert.ToInt32(vec3[2]);

        PacketPoolManager.inst.OnBlackhatUpdateStrategy();
    }

    public void setNetworkState(string data) {
        Debug.Log("Changed State Recieved");
        Shared.inst.gameState.currentState = (SharedGameStates)Convert.ToInt32(data);

    }

    public void addNewPlayers(string data) {

        string[] ids = data.Split(',');

        if (SharedPlayer.playerIdForThisDevice == -1) {
            SharedPlayer.playerIdForThisDevice = int.Parse(ids[ids.Length - 2]);
        }

        for (int i = 0; i < ids.Length; i++) {
            if (ids[i].Length != 0) {
                int netID = int.Parse(ids[i]);
                Shared.inst.getOrAddPlayerById(netID);
            }


        }



    }

    public void setPlayerSettings(string data) {
        string[] settings = data.Split(',');
        int playerID = int.Parse(settings[0]);
        string username = settings[1];
        bool isReady = bool.Parse(settings[2]);
        int dropValue = int.Parse(settings[3]);

        if (playerID == -1)
            return;

        SharedPlayer player = Shared.inst.getOrAddPlayerById(playerID);
        player.username = username;
        player.isReady = isReady;
        player.role = dropValue;
        // id,name,ready,role

        LobbyMenuManager.inst.OnSettingsChanged(playerID);
    }

    public void setPlayerIsReady(string csv) {
        string[] data = csv.Split(',');
        int playerID = int.Parse(data[0]);
        bool isReady = bool.Parse(data[1]);


        SharedPlayer player = Shared.inst.getOrAddPlayerById(playerID);
        if (playerID == -1)
            return;

        player.isReady = isReady;
    }

    public void changePacketLifecycleStatus(string data) {
        string[] tokens = data.Split(',');
        int packetID = int.Parse(tokens[0]);
        PACKET_LIFECYCLE_STATUS newStatus = (PACKET_LIFECYCLE_STATUS)int.Parse(tokens[1]);
        SimpleEnemyController sec = PacketPoolManager.inst.find(packetID);
        sec.status = newStatus;
    }
}
