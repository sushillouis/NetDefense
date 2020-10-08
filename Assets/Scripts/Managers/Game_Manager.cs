using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;



public class Game_Manager : MonoBehaviour {

    //Singleton
    public static Game_Manager inst;


    //Range of each trait for the level
    public static int COLOR_COUNT = 3;
    public static int SHAPE_COUNT = 3;
    public static int SIZE_COUNT = 3;

    //Interval at which packets are spawned in seconds
    public float spawnInterval;
    private float lastSpawn;

    //All paths in the level
    public Destination[] destinations;

    //Used to assign traits of packets. Length must be that same as static variables above.
    public Material[] materials;
    public Mesh[] models;
    public float[] sizes;

    //Parent object of the two canvases
    public GameObject UI;
    //Reference to each UI canvas
    private GameObject whiteHatUI, blackHatUI;

    public int match_length = 60;
    private float start_time;

    public GameObject GraphWidget;
    public float graph_update_time = 1;
    private float start_graph_update_time;

    public Text countdownText;
    public float maxTime = 5;
    public float startTimeCountdown;
    public bool hasStartedCountdown;

    public bool poolHasSpawned;

    public bool hasLazyInitEntityManagerNetworkField;

    public NetworkManagerHLAPI network;
    public GameObject poolManager;
    public GameObject entityManager;

    public void Awake() {
        inst = this;
    }

    void Start() {
        lastSpawn = Time.time;
        start_graph_update_time = Time.time;


        inst = this;
        whiteHatUI = UI.transform.GetChild(0).gameObject;
        blackHatUI = UI.transform.GetChild(1).gameObject;



        if (!MainMenu.isMultiplayerSelectedFromMenu) {
            /* Shared.inst.getDevicePlayer().username = "Player 1";
             Shared.inst.getDevicePlayer().role = SharedPlayer.WHITEHAT;
             Debug.Log("Tried making device player...");*/
            network.LauchHost(false);
            Shared.inst.getDevicePlayer().role = SharedPlayer.WHITEHAT;
            Shared.inst.getDevicePlayer().username = "You";
        }
    }

    public void forceNetworkIdentiesAwake() {
        poolManager.SetActive(true);
        entityManager.SetActive(true);
        if (!hasLazyInitEntityManagerNetworkField) {
            EntityManager.inst.isMultiplayer = EntityManager.inst.isMultiplayer || MainMenu.isMultiplayerSelectedFromMenu;
            hasLazyInitEntityManagerNetworkField = true;
        }
    }

    void Update() {

        // [Jonathon] Can this only be checked when the shortcut to switch roles is pressed?
        // [Teddy] yes that's a better spot for it, much better haha

        forceNetworkIdentiesAwake();


        // check to see if it's time to change from lobby to countdown
        if (Shared.inst.gameState.currentState == SharedGameStates.LOBBY && EntityManager.inst.isServer && LobbyMenuManager.inst.hasPressedBegin) {
            Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_NETWORK_STATE, ((int)SharedGameStates.COUNTDOWN) + ""));
        }

        if (Shared.inst.gameState.currentState == SharedGameStates.COUNTDOWN) {
            UpdateCountdownState();
        }


        if (Shared.inst.gameState.currentState == SharedGameStates.PLAY) {
            UpdateGamePlayState();
        }

        if (Shared.inst.gameState.currentState == SharedGameStates.OVER) {
            UpdateGameOverState();
        }


    }

    public void UpdateGameOverState() {
        ScoreManager.inst.CalculateEndGameStats();

        if (Shared.inst.getDevicePlayer().playerID == 0)
            Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_METRICS,
                ScoreManager.inst.totalFriendlyPacketsSpawned
                + "," + ScoreManager.inst.totalBadPacketsSpawned
                + "," + ScoreManager.inst.totalPacketsSpawned));

        GameOverMenuManager.inst.OnRefreshMenu();
    }

    public void UpdateCountdownState() {
        if (!hasStartedCountdown) {
            startTimeCountdown = Time.time;
            hasStartedCountdown = true;
        }
        float elapsed = Time.time - startTimeCountdown;
        countdownText.text = (int)(1 + (maxTime - elapsed)) + "";
        if (elapsed > maxTime) {
            Shared.inst.gameState.currentState = SharedGameStates.PLAY;
            if (EntityManager.inst.isServer)
                Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_NETWORK_STATE, ((int)Shared.inst.gameState.currentState) + ""));
        }
    }

    public void UpdateGamePlayState() {


        float elapsed1 = Time.time - lastSpawn;
        float elapsed2 = Time.time - start_time;
        float elapsed3 = Time.time - start_graph_update_time;



        int unspawnedPackets = PacketPoolManager.inst.getUndeployedPacketCount();
        WhiteHatMenu.inst.OnTimerChange(unspawnedPackets);


        if (elapsed3 > graph_update_time) {
            // update graph
            GraphWidget.GetComponent<WidgetGraph>().UpdateDataSet(Shared.inst.getDevicePlayer().role == SharedPlayer.WHITEHAT ? (int)Shared.inst.gameMetrics.derrivative_whitehat_score : (int)Shared.inst.gameMetrics.derrivative_blackhat_score);
            start_graph_update_time = Time.time;
        }

        //Check if new packet should be spawned
        if (elapsed1 > spawnInterval) {
            RequestPacketSpawnFromPool();

            lastSpawn = Time.time;
        }


        if (poolHasSpawned && PacketPoolManager.inst.getAlivePackets() == 0) {
            if (!MainMenu.isMultiplayerSelectedFromMenu)
                Shared.inst.gameState.currentState = SharedGameStates.OVER;
            else if (EntityManager.inst.isServer)
                Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_NETWORK_STATE, ((int)(Shared.inst.gameState.currentState = SharedGameStates.OVER)) + ""));
        }

    }

    public void RequestPacketSpawnFromPool() {
        POPULATE_POOL_ERROR_CODES pool_status = POPULATE_POOL_ERROR_CODES.SUCCESS;
        if (!poolHasSpawned)
            pool_status = PacketPoolManager.inst.populatePool();

        if (pool_status == POPULATE_POOL_ERROR_CODES.SUCCESS) {
            PacketPoolManager.inst.deployNextPacket();
            poolHasSpawned = true;
        } else
            Debug.Log("POOL STATUS... " + pool_status);

        if (!MainMenu.isMultiplayerSelectedFromMenu && (pool_status == POPULATE_POOL_ERROR_CODES.WAITING_ON_BLACKHAT_DEFINITION || pool_status == POPULATE_POOL_ERROR_CODES.WAITING_ON_BLACKHAT_TARGET_SELECTION)) {
            BlackHatNPC.inst.setBlackhatNPCvalues();
        }
    }

    //Set traits of packet based on the values it already holds
    public void SetTraits(SimpleEnemyController packet) {
        Material[] tempMaterials = packet.GetComponent<MeshRenderer>().materials;
        switch (packet.color) {
            case 0:
                tempMaterials[0] = materials[0];
                break;
            case 1:
                tempMaterials[0] = materials[1];
                break;
            case 2:
                tempMaterials[0] = materials[2];
                break;
        }
        packet.GetComponent<MeshRenderer>().materials = tempMaterials;

        switch (packet.size) {
            case 0:
                packet.transform.localScale = new Vector3(sizes[0], sizes[0], sizes[0]);
                break;
            case 1:
                packet.transform.localScale = new Vector3(sizes[1], sizes[1], sizes[1]);
                break;
            case 2:
                packet.transform.localScale = new Vector3(sizes[2], sizes[2], sizes[2]);
                break;
        }

        switch (packet.shape) {
            case 0:
                packet.GetComponent<MeshFilter>().mesh = models[0];
                break;
            case 1:
                packet.GetComponent<MeshFilter>().mesh = models[1];
                break;
            case 2:
                packet.GetComponent<MeshFilter>().mesh = models[2];
                break;
        }
    }
}
