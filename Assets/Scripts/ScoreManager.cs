using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PacketLifeStatus {
    SUCCESSFUL,
    FAIL,
    UNCERTAIN
}

[System.Serializable]
public class PacketProfile {
    public int size;
    public int color;
    public int shape;

    public PacketProfile(int size, int color, int shape) {
        this.size = size;
        this.color = color;
        this.shape = shape;
    }
}

public static class PacketProfileTypes {
    public static PacketProfile[] profiles = new PacketProfile[3 * 3 * 3];

    static PacketProfileTypes() {
        int index = 0;
        for (int size = 0; size < 3; size++) {
            for (int color = 0; color < 3; color++) {
                for (int shape = 0; shape < 3; shape++) {
                    profiles[index++] = new PacketProfile(size, color, shape);
                }
            }
        }
    }
}


[System.Serializable]
public class PacketCompletedMetric {
    public int id;
    public bool wasMalic;
    public PacketLifeStatus status;

    public PacketCompletedMetric(int id, bool wasMalic, PacketLifeStatus status) {
        this.id = id;
        this.wasMalic = wasMalic;
        this.status = status;
    }
}

public class ScoreManager : MonoBehaviour {
    public static ScoreManager inst;
	public void Awake() { inst = this; }

    public int updatesGivenPerWave;

    public Text score_black;
    public Text score_white;


    // metrics to calculate a fair score between players (READ ONLY)
    public int scoreFactor;
    public int totalPacketsSpawned;
    public int totalBadPacketsSpawned;
    public int totalFriendlyPacketsSpawned;
    public int badPacketSuccesses;
    public int friendlyPacketSuccesses;
    public float fraction_black_hat_score;
    public float fraction_white_hat_score;

    // public int black_hat_score_total_for_end_game;
    // public int white_hat_score_total_for_end_game;

    public float white_score_derivative;
    public float black_score_derivative;

    public float white_score;
    public float black_score;

    public float update_score_rate; // update score by rate of change every n seconds
    public float timer;

    public int maxPacketsConsidered; // history size
    public List<PacketCompletedMetric> packetMetrics; // packet lifecycle history about last n instances
    public List<PacketProfile> packetTypeHistory;
    public List<KeyValuePair<PacketProfile, int>> histogram;
    public List<KeyValuePair<float, float>> badPacketsFilteredHistory;

    public int getFrequencyOfProfile(PacketProfile profile) {
        int frequency = 0;
        foreach (PacketProfile pp in packetTypeHistory)
            if (profile.color == pp.color && profile.shape == pp.shape && profile.size == pp.size)
                frequency++;

        return frequency;
    }

    public void updateHistogram() {
        histogram.Clear();
        for (int i = 0; i < 3 * 3 * 3; i++)
            histogram.Add(new KeyValuePair<PacketProfile, int>(PacketProfileTypes.profiles[i], getFrequencyOfProfile(PacketProfileTypes.profiles[i])));

		histogram.Sort(delegate (KeyValuePair<PacketProfile, int> e1, KeyValuePair<PacketProfile, int> e2) {
			return e2.Value.CompareTo(e1.Value);
        });
    }

    public PacketCompletedMetric getMetricById(int id) {
        foreach (PacketCompletedMetric metric in packetMetrics)
            if (metric.id == id)
                return metric;

        return null;
    }

    public void Start() {
        Shared.inst.gameMetrics.whitehat_cash = 1000;
        WhiteHatMenu.inst.OnCashChanged();
        packetMetrics = new List<PacketCompletedMetric>();
        packetTypeHistory = new List<PacketProfile>();
        histogram = new List<KeyValuePair<PacketProfile, int>>();
        badPacketsFilteredHistory = new List<KeyValuePair<float, float>>();

        timer = Time.time;


        if (MainMenu.difficulty == Difficulty.EASY)
            updatesGivenPerWave = 2;
        else if (MainMenu.difficulty == Difficulty.MEDIUM || MainMenu.difficulty == Difficulty.HARD)
            updatesGivenPerWave = 1;
    }

    public void Update() {

        if (GameManager.inst.isBetweenWaves || Shared.inst.gameState.currentState == SharedGameStates.OVER)
            return;

        if (packetMetrics.Count < 3)
            return;

        // generate score using metrics history
        int badPacketSuccesses = 0;
        int totalbadPackets = 0;
        foreach (PacketCompletedMetric metric in packetMetrics) {
            if (metric.wasMalic && metric.status == PacketLifeStatus.SUCCESSFUL)
                badPacketSuccesses++;

			if (metric.wasMalic)
                totalbadPackets++;
        }

        if (totalbadPackets == 0)
            return;

        if ((Time.time - timer) > update_score_rate) {
			// There is no point in calculating this stuff if enouph time hasn't elapsed
			fraction_black_hat_score = (badPacketSuccesses / (float)totalbadPackets);
	        fraction_white_hat_score = (1f - fraction_black_hat_score);

	        white_score_derivative = (fraction_white_hat_score * scoreFactor);
	        black_score_derivative = (fraction_black_hat_score * scoreFactor);

			// Add the derivative to the score
            white_score += white_score_derivative;
            black_score += black_score_derivative;
            timer = Time.time;
        }
    }

    public void OnEnteredBetweenWavesState() {
        WhiteHatNPC.inst.OnBetweenWaves();
        BlackHatNPC.inst.OnBetweenWaves();

		// Show end of wave screen
        DismissableScreenManager.inst.mainContent.isValid = true;

		// Give more updates to each router
        GameObject[] routers = GameObject.FindGameObjectsWithTag("Router");
        foreach (GameObject go in routers) {
            Router r = go.GetComponent<Router>();
            if (r != null)
                r.updatesRemaining += updatesGivenPerWave;
        }
        RouterManager.inst.settings.SetActive(false);
    }

    public void OnFriendlyPacketTransfered(int instanceID) {
        friendlyPacketSuccesses++;
        PacketCompletedMetric m = getMetricById(instanceID);
        if (m != null)
            m.status = PacketLifeStatus.SUCCESSFUL;

		OnMetricsUpdated();
    }

    public void OnBadPacketTransfered(int instanceID) {
        badPacketSuccesses++;
        PacketCompletedMetric m = getMetricById(instanceID);
        if (m != null)
            m.status = PacketLifeStatus.SUCCESSFUL;

        OnMetricsUpdated();
    }

    public void OnFriendlyPacketSpawned(int instanceID) {
        totalFriendlyPacketsSpawned++;
        packetMetrics.Add(new PacketCompletedMetric(instanceID, false, PacketLifeStatus.FAIL));
        if (packetMetrics.Count > maxPacketsConsidered) {
            packetMetrics.RemoveAt(0);
        }
        OnMetricsUpdated();
    }

    public void OnBadPacketSpawned(int instanceID) {
        totalBadPacketsSpawned++;
        packetMetrics.Add(new PacketCompletedMetric(instanceID, true, PacketLifeStatus.FAIL));
        if (packetMetrics.Count > maxPacketsConsidered) {
            packetMetrics.RemoveAt(0);
        }
        OnMetricsUpdated();
    }

    public void UpdateUIClient() {
        if (Shared.inst.gameMetrics.blackhat_score != 0)
            score_black.text = Shared.inst.gameMetrics.blackhat_score + "";

        if (Shared.inst.gameMetrics.whitehat_score != 0)
            score_white.text = Shared.inst.gameMetrics.whitehat_score + "";
    }

    public void OnMetricsUpdated() {
		// Calculate total number of packets that have spawned
		totalPacketsSpawned = totalBadPacketsSpawned + totalFriendlyPacketsSpawned;

        // for plot of total spawned vs filtered
        badPacketsFilteredHistory.Add(new KeyValuePair<float, float>(totalBadPacketsSpawned, badPacketSuccesses));

        if (!MainMenu.isMultiplayerSelectedFromMenu) {
            Shared.inst.gameMetrics.whitehat_score = (int)white_score;
            Shared.inst.gameMetrics.blackhat_score = (int)black_score;
            Shared.inst.gameMetrics.derrivative_whitehat_score = (int)white_score_derivative;
            Shared.inst.gameMetrics.derrivative_blackhat_score = (int)black_score_derivative;
        }

        if (EntityManager.inst.isMultiplayer && EntityManager.inst.isServer) {
            Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_SCORES, (int)black_score + "," + (int)white_score));
            Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_SCORE_DERRIVATIVES, (int)black_score_derivative + "," + (int)white_score_derivative));
        }

		// update ui here
        UpdateUIClient();
    }
}
