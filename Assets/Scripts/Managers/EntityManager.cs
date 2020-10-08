using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class EntityManager : NetworkBehaviour {

    public string[] options = { "LEFT", "RIGHT", "CENTRE" };

    public static EntityManager inst;
    //   private List<SimpleEnemyController> packets;

    public GameObject packetPrefab;


    public bool isMultiplayer;

    private GameObject cmdPacket;

    public void Awake() {
        inst = this;
    }

    void Start() {
        isMultiplayer = MainMenu.isMultiplayerSelectedFromMenu;
        Debug.Log("Multiplayer Selection is... " + isMultiplayer);
    }

    public void Update() {

        foreach (GameObject p in PacketPoolManager.inst.packets) {
            p.GetComponent<SimpleEnemyController>().Tick();
        }
    }







    public void DeletePacketMultiplayer(SimpleEnemyController packet) {
        if (isMultiplayer) {
            deleteMultiplayer(packet);
        }
    }

    private void deleteMultiplayer(SimpleEnemyController packet) {
        NetworkServer.Destroy(packet.gameObject);
    }


}
