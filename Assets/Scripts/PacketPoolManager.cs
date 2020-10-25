﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;




public enum POPULATE_POOL_ERROR_CODES {
    SUCCESS,
    WAITING_ON_BLACKHAT_DEFINITION,
    WAITING_ON_BLACKHAT_TARGET_SELECTION,
    MIX_MATCH_EXCEPTION // @targetting_probabilites should match @packet_destinations array length

}

public class PacketPoolManager : NetworkBehaviour {
    public static PacketPoolManager inst;

    public string[] packet_destinations;

    public int N_PER_TYPE;
    public float badSpawnProbability = 0.5f;

    public List<GameObject> packets;

    public GameObject packetPrefab;

    private int deployIndex;

    private void Awake() {
        inst = this;
        packets = new List<GameObject>();
    }

    public POPULATE_POOL_ERROR_CODES populatePool() {

        if (!Shared.inst.blackhatHasDefinedABadPacket()) {
            return POPULATE_POOL_ERROR_CODES.WAITING_ON_BLACKHAT_DEFINITION;
        }

        if (!Shared.inst.blackhatHatChosenTargetRatios()) {
            return POPULATE_POOL_ERROR_CODES.WAITING_ON_BLACKHAT_TARGET_SELECTION;
        }

        //packets.Clear();
        for (int i = 0; i < N_PER_TYPE; i++)
            for (int color = 0; color < Game_Manager.COLOR_COUNT; color++)
                for (int shape = 0; shape < Game_Manager.SHAPE_COUNT; shape++)
                    for (int size = 0; size < Game_Manager.SIZE_COUNT; size++) {

                        bool isBad = Shared.inst.isBadPacket(color, shape, size);

                        string target = packet_destinations[Random.Range(0, packet_destinations.Length)];

                        Debug.Assert(target != null, "Target not selected");

                        if (isBad) {
                            if (Shared.inst.gameMetrics.target_probabilities.Count != Game_Manager.inst.destinations.Length) {
                                Debug.LogError("Check Array Sizes");
                                return POPULATE_POOL_ERROR_CODES.MIX_MATCH_EXCEPTION;
                            }

                            // TODO GENERALIZE FOR n TARGETS
                            //target = getTarget();

                            continue;
                        }

                        float randomProb = Random.value;
                        if (badSpawnProbability > randomProb)
                            if (MainMenu.isMultiplayerSelectedFromMenu)
                                CmdCreatePacket(Shared.inst.maliciousPacketProperties.color, Shared.inst.maliciousPacketProperties.size, Shared.inst.maliciousPacketProperties.shape, target, true);
                            else
                                createPacket(Shared.inst.maliciousPacketProperties.color, Shared.inst.maliciousPacketProperties.size, Shared.inst.maliciousPacketProperties.shape, target, true);



                        if (MainMenu.isMultiplayerSelectedFromMenu)
                            CmdCreatePacket(color, size, shape, target, isBad);
                        else
                            createPacket(color, size, shape, target, isBad);


                    }



        return POPULATE_POOL_ERROR_CODES.SUCCESS;
    }

    public string getTarget() {
        string target;
        float p1 = Shared.inst.gameMetrics.target_probabilities["LEFT"];
        float p2 = Shared.inst.gameMetrics.target_probabilities["RIGHT"];
        float p3 = Shared.inst.gameMetrics.target_probabilities["CENTRE"];


        float probability = Random.value;

        if (probability <= p1) {
            target = "LEFT";
        } else if (probability <= p2 + p1) {
            target = "RIGHT";
        } else {
            target = "CENTRE";
        }

        return target;
    }


    public void OnBlackhatUpdateStrategy() {
        foreach (GameObject pac in GameObject.FindGameObjectsWithTag("Packet")) {
            SimpleEnemyController sec = pac.GetComponent<SimpleEnemyController>();
            if (sec.status != PACKET_LIFECYCLE_STATUS.UNSPAWNED) {
                continue;
            }

            bool malic = Shared.inst.isBadPacket(sec.color, sec.shape, sec.size);
            sec.malicious = malic;
            sec.destination = malic ? Destination.getDestinationByID(getTarget()) : Destination.getDestinationByID(packet_destinations[Random.Range(0, packet_destinations.Length)]);
            int index = Random.Range(0, sec.destination.paths.Count);
            sec.path = sec.destination.paths[index];
            sec.SetSpawnRotandPos();
            // sec.StartCoroutine("ClearPulseColor");

            if (malic) {
                sec.StartCoroutine("Pulsate");
            } else {
                sec.StopCoroutine("Pulsate");
            }
        }
    }

    public void CleanUpStatics() {
        foreach (GameObject go in packets) {
            Destroy(go);
        }

        packets.Clear();
    }

    public GameObject createPacket(int color, int size, int shape, string destination, bool malicious) {
        GameObject packet = Instantiate(packetPrefab);
        SimpleEnemyController sec = packet.GetComponent<SimpleEnemyController>();

        
        sec.destination = Destination.getDestinationByID(destination);
        sec.setupBehavior(color, size, shape, malicious);

        Debug.Assert(sec.destination != null, "Dest is null :(");

        packets.Add(packet);
        packet.transform.parent = transform;

        if (EntityManager.inst.isMultiplayer) {
            NetworkServer.Spawn(packet);
        }

        return packet;
    }

    [Command]
    public void CmdCreatePacket(int color, int size, int shape, string destination, bool malicious) {
        createPacket(color, size, shape, destination, malicious);
    }

    public void deployNextPacket() {
        if (deployIndex > packets.Count - 1) {

            return;
        }
        SimpleEnemyController pac = packets[deployIndex++].GetComponent<SimpleEnemyController>();

        if (!pac.malicious)
            ScoreManager.inst.OnFriendlyPacketSpawned(pac.id);
        else
            ScoreManager.inst.OnBadPacketSpawned(pac.id);

        pac.status = PACKET_LIFECYCLE_STATUS.ENROUTE;
        pac.OnDeployed();

    }

    public int getPoolSize() {
        return packets.Count == 0 ? GameObject.FindGameObjectsWithTag("Packet").Length : packets.Count;
    }

    public int getUndeployedPacketCount() {
        return getPoolSize() - deployIndex;
    }

    public int getAlivePackets() {

        int count = 0;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Packet")) {
            if (go.GetComponent<SimpleEnemyController>().status == PACKET_LIFECYCLE_STATUS.UNSPAWNED
                || go.GetComponent<SimpleEnemyController>().status == PACKET_LIFECYCLE_STATUS.ENROUTE)
                count++;

        }

        return count;
    }

    public SimpleEnemyController find(int id) {


        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Packet")) {
            if (go.GetComponent<SimpleEnemyController>().id == id)
                return go.GetComponent<SimpleEnemyController>();
        }




        return null;
    }
}