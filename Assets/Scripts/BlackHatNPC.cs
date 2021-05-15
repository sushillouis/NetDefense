using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHatNPC : MonoBehaviour {

    public int attackCount = -1;

	// Range of values used to determine how many times a malicous packet should get filtered before the AI catches on
	public Vector2Int negativesInRowBeforeUpdateRange = new Vector2Int(4, 7);
	// The actual number of times (within the above range) that a packet should get filtered before the AI catches on
	[SerializeField]
	private int negativesInRowBeforeUpdate = 7;
	// The number of times a malicous packet has been filtered
	[SerializeField]
	private int negativesInRow = 0;

    [SerializeField]
    private string[] targets;

    [SerializeField]
    private float[] values;

    public static BlackHatNPC inst;

    private void Awake() {
        inst = this;
		negativesInRowBeforeUpdate = Random.Range(negativesInRowBeforeUpdateRange.x, negativesInRowBeforeUpdateRange.y);
        if ((MainMenu.hat == SharedPlayer.BLACKHAT && !MainMenu.isMultiplayerSelectedFromMenu) || MainMenu.isMultiplayerSelectedFromMenu) {
            Destroy(this);
        }
    }

    public void setBlackhatNPCvalues() {

        if (MainMenu.isMultiplayerSelectedFromMenu) return; // TODO: Is this nessicary since the manager is deleted on awake in this case?

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
            if (!tp.ContainsKey(targets[i]))
                tp.Add(targets[i], values[i]);
            else
                tp[targets[i]] = values[i];
        }
    }

    public void OnBetweenWaves() {
        if (MainMenu.hat == SharedPlayer.WHITEHAT)
            setBlackhatNPCvalues();
    }


	// Called whenever a packet hits a router or destination
	public void OnMaliciousPacketCollision(bool reachedDestination) {
		negativesInRow = IntMath.Clamp(negativesInRow + (reachedDestination ? -1 : 1), 0, negativesInRowBeforeUpdate);

		if(negativesInRow >= negativesInRowBeforeUpdate){
			// Generate a new set of bad packet properties
			setBlackhatNPCvalues();
			// Reset counter and update the check for next time to be within the range
			negativesInRow = 0;
			negativesInRowBeforeUpdate = Random.Range(negativesInRowBeforeUpdateRange.x, negativesInRowBeforeUpdateRange.y);
		}
	}

    void Update() {
		
    }
}
