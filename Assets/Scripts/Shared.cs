using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SharedPlayer {

    public static int BLACKHAT = 1;
    public static int WHITEHAT = 2;

    public string username = "...";

    public int playerID = -1;
    public int role = -1;

    public bool isReady;
    public bool isHost;

    public static int playerIdForThisDevice = -1;

}

[System.Serializable]
public class SyncEvent {
    public int id;
    public string data;

    public SyncEvent(int id, string data) {
        this.id = id;
        this.data = data;
    }
}

public class Shared : MonoBehaviour {
    [Header("Synchronized Values")]
    [Space(5)]

    public MaliciousPacketProperties maliciousPacketProperties;
    [Space(20)]
    public GameMetrics gameMetrics;

    [Space(20)]

    public GameState gameState;



    [HideInInspector]
    public static Shared inst;

    [Space(20)]
    public List<SyncEvent> syncEvents;

    [Space(20)]
    public List<SharedPlayer> players;

    public void Awake() {
        inst = this;
        inst.gameState = new GameState();
        syncEvents = new List<SyncEvent>();
        players = new List<SharedPlayer>();


    }

    private void Start() {

        if (MainMenu.isMultiplayerSelectedFromMenu) {
            inst.gameState.currentState = SharedGameStates.LOBBY;
        } else {
            inst.gameState.currentState = SharedGameStates.COUNTDOWN;

        }



    }

    public SharedPlayer getOrAddPlayerById(int id) {
        if (id == -1)
            id++;

        foreach (SharedPlayer n in players) {
            if (n.playerID == id) {
                return n;
            }
        }


        SharedPlayer newPlayer = new SharedPlayer();
        newPlayer.playerID = id;
        newPlayer.isHost = id == 0;
        players.Add(newPlayer);

        if (MainMenu.isMultiplayerSelectedFromMenu)
            LobbyMenuManager.inst.OnNewPlayerConnected(newPlayer);

        return newPlayer;

    }

    public string getIdString() {
        string ids = "";
        foreach (SharedPlayer n in players) {
            ids += n.playerID + ",";
        }
        return ids;
    }

    public SharedPlayer getDevicePlayer() {
        return getOrAddPlayerById(SharedPlayer.playerIdForThisDevice);
    }

    public bool blackhatHasDefinedABadPacket() {
        return inst.maliciousPacketProperties.color != -1 && inst.maliciousPacketProperties.size != -1 && inst.maliciousPacketProperties.shape != -1;
    }

    public void unReadyPlayers() {
        foreach (SharedPlayer p in inst.players) {
            if (MainMenu.isMultiplayerSelectedFromMenu)
                inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_PLAYERS_READY_STATUS, p.playerID + "," + false));
            else
                p.isReady = false;
        }
    }

    public void setPlayerToReady(int playerID) {
        SharedPlayer p = getOrAddPlayerById(playerID);
        if (MainMenu.isMultiplayerSelectedFromMenu)
            inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_PLAYERS_READY_STATUS, p.playerID + "," + true));
        else
            p.isReady = true;
    }

	public bool isPlayerReady(int playerID){
		foreach (SharedPlayer p in inst.players) {
			if(p.playerID == playerID)
				return p.isReady;
		}

		return false;
	}

    public int getPlayersReady() {
        int count = 0;
        foreach (SharedPlayer p in inst.players) {
            if (p.isReady)
                count++;
        }

        return count;
    }

    public bool allPlayersReady() {
        return getPlayersReady() == inst.players.Count;
    }

    public bool blackhatHatChosenTargetRatios() {

        // search targetting ratios for a non zero value.
        foreach (string target_key in inst.gameMetrics.target_probabilities.Keys) {
            if (inst.gameMetrics.target_probabilities[target_key] != 0)
                return true;
        }

        return false;
    }

	public bool blackHatChosenMaliciousPacketProperties(){
		return maliciousPacketProperties.shape >= 0 && maliciousPacketProperties.size >= 0 && maliciousPacketProperties.color >= 0;
	}

    public bool isBadPacket(int color, int shape, int size) {
        return color == inst.maliciousPacketProperties.color && shape == inst.maliciousPacketProperties.shape && size == inst.maliciousPacketProperties.size;
    }

    public void OnPauseButtonSelected() {
        inst.gameState.currentState = SharedGameStates.PAUSE;
        if (MainMenu.isMultiplayerSelectedFromMenu)
            Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_NETWORK_STATE, ((int)SharedGameStates.PAUSE).ToString()));
    }

    public void OnResumeButtonPressed() {
        inst.gameState.currentState = SharedGameStates.PLAY;
        Time.timeScale = 1;
        if (MainMenu.isMultiplayerSelectedFromMenu)
            Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_NETWORK_STATE, ((int)SharedGameStates.PLAY).ToString()));

    }

    public void OnQuitButtonPressed(string sceneName = "MainMenu") {


        // reset global state
        // PacketPoolManager.inst.CleanUpStatics();
        ScoreManager.inst.packetMetrics.Clear();
        Destination.CleanUpStatics();
        SharedPlayer.playerIdForThisDevice = -1;
        NetworkManagerHLAPI.reset();
        inst.players.Clear();
        inst.syncEvents.Clear();
        Destroy(inst);
        Destroy(LobbyMenuManager.inst);

        foreach (BarchartManager barchats in BarchartManager.insts)
            Destroy(barchats);
        BarchartManager.insts.Clear();
        Destroy(DismissableScreenManager.inst);

        // possilby redundant and wet code
        if (NetworkManager.singleton != null) {

            NetworkManager.singleton.StopClient();
            NetworkManager.singleton.StopHost();
            NetworkManager.singleton.StopServer();
            NetworkManager.singleton.StopAllCoroutines();
            Destroy(NetworkManager.singleton);
        }
        Time.timeScale = 1;

        if (MainMenu.isMultiplayerSelectedFromMenu) {
            inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_NETWORK_STATE, ((int)SharedGameStates.OFFLINE).ToString()));
            inst.syncEvents.Add(new SyncEvent(MessageTypes.FORCE_GAME_SHUTDOWN, ""));
            SceneManager.LoadScene(sceneName);
        } else
            SceneManager.LoadScene(sceneName);
    }
}

public enum SharedGameStates {
    OFFLINE,
    NONE,
    LOBBY,
    COUNTDOWN,
    PLAY,
    OVER,
    PAUSE

}

[System.Serializable]
public class GameState {

    [SerializeField]
    private SharedGameStates _currentState;

    public SharedGameStates lastState;

    public SharedGameStates currentState {

        set {
            if (lastState != _currentState)
                lastState = _currentState;

            _currentState = value;
            UIManager.inst.OnGameStateChanged(value);
        }

        get {
            return _currentState;
        }

    }
}


[System.Serializable]
public class MaliciousPacketProperties {

    public int size = -1, color = -1, shape = -1;

    [Space(25)]

    public int spawningIndex;

}

[System.Serializable]
public class GameMetrics {

    public bool isWhiteHat;
    public int whitehat_cash;

    public int whitehat_score;
    public int blackhat_score;

    public float derrivative_whitehat_score;
    public float derrivative_blackhat_score;

	public ulong startTime = 0;
	public ulong endTime = 0;

    public float[] metrics; // metrics for end of game report

    public Dictionary<string, float> target_probabilities = new Dictionary<string, float>();

}
