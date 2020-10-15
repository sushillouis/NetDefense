﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHatNPC : MonoBehaviour {
    public int attackCount = -1;

    [SerializeField]
    private string[] targets;

    [SerializeField]
    private float[] values;

    public static BlackHatNPC inst;

    private void Awake() {
        inst = this;
    }

    public void setBlackhatNPCvalues() {

        if (!MainMenu.isMultiplayerSelectedFromMenu) {
            targets = PacketPoolManager.inst.packet_destinations;
            values = new float[targets.Length];
            Shared.inst.maliciousPacketProperties.color = Random.Range(0, Game_Manager.COLOR_COUNT);
            Shared.inst.maliciousPacketProperties.size = Random.Range(0, Game_Manager.SIZE_COUNT);
            Shared.inst.maliciousPacketProperties.shape = Random.Range(0, Game_Manager.SHAPE_COUNT);

            attackCount = Random.Range(1, targets.Length + 1);
            List<int> targettingIndcies = new List<int>();
            for (int i = 0; i < targets.Length; i++) {
                targettingIndcies.Add(i);
            }

            for (int i = 0; i < attackCount; i++) {
                int randomSample = Random.Range(0, targettingIndcies.Count);
                int randomIndex = targettingIndcies[randomSample];
                values[randomIndex] = 1f / attackCount;
                targettingIndcies.RemoveAt(randomSample);
            }

            Dictionary<string, float> tp = Shared.inst.gameMetrics.target_probabilities;

            for (int i = 0; i < values.Length; i++) {
                tp.Add(targets[i], values[i]);
            }
        }
    }

    void Update() {

    }
}
