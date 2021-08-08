using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ScoreManager : Core.Utilities.SingletonPun<ScoreManager> {
	// Callback for score events
	public delegate void ScoreEventCallback(float whiteHatDerivative, float whiteHatScore, float blackHatDerivative, float blackHatScore);
	public static ScoreEventCallback scoreEvent;

	// Enum storing the type of score event to process
	[System.Serializable]
	public enum ScoreEvent {
		MaliciousDestroyed,
		MaliciousSuccess,
		GoodDestroyed,
		GoodSuccess
	}

	// Class representing the metrics used throughout a wave
	public class WaveMetrics {
		public int successfulMaliciousPackets = 0, totalMaliciousPackets = 0;
		public int successfulGoodPackets = 0, totalGoodPackets = 0;

		public override string ToString(){
			return "Malicious: " + successfulMaliciousPackets + "/" + totalMaliciousPackets + ", Good: " + successfulGoodPackets + "/" + totalGoodPackets;
		}
	}

	// Weights for each side (the amount of points they get per score event if they are 100% successful)
	public float blackHatMaxScorePerEvent = 50;
	public float whiteHatMaxScorePerEvent = 50;
	// The penalty when a whitehat filters out a good packet
	public float scorePenaltyForWhiteHatMistake = 100;

	// The score of each side
	public float blackHatScore = 0;
	public float whiteHatScore = 0;

	// List of metrics for each wave
	List<WaveMetrics> waveMetrics = new List<WaveMetrics>();

	// When we are dis/enabled un/register ourselves as a wave start listener
	void OnEnable(){ GameManager.waveStartEvent += OnWaveStart; }
	void OnDisable(){ GameManager.waveStartEvent -= OnWaveStart; }

	// Function which gets the wave metrics for the current wave
	public WaveMetrics getCurrentWaveMetrics(){
		return waveMetrics.LastOrDefault();
	}

	// Function which totals all of the wave metrics
	public WaveMetrics getAllWavesMetrics(){
		WaveMetrics ret = new WaveMetrics();

		foreach(WaveMetrics m in waveMetrics){
			ret.successfulMaliciousPackets += m.successfulMaliciousPackets;
			ret.totalMaliciousPackets += m.totalMaliciousPackets;
			ret.successfulGoodPackets += m.successfulGoodPackets;
			ret.totalGoodPackets += m.totalGoodPackets;
		}

		return ret;
	}


	// -- Callbacks --


	// Function adds a new wave metrics when a wave starts
	void OnWaveStart(){
		// Add another wave's worth of information to the wave metrics
		waveMetrics.Add(new WaveMetrics());
	}

	// Function which processes a score event sent from a packet (network synced)
	public void ProcessScoreEvent(ScoreEvent _event) { photonView.RPC("RPC_ScoreManager_ProcessScoreEvent", RpcTarget.AllBuffered, _event); }
	[PunRPC] void RPC_ScoreManager_ProcessScoreEvent(ScoreEvent _event){
		// Update the metrics
		WaveMetrics metrics = getCurrentWaveMetrics();
		switch(_event){
			case ScoreEvent.MaliciousDestroyed:
				metrics.totalMaliciousPackets++;
				break;
			case ScoreEvent.MaliciousSuccess:
				metrics.successfulMaliciousPackets++;
				metrics.totalMaliciousPackets++;
				break;
			case ScoreEvent.GoodDestroyed:
				metrics.totalGoodPackets++;
				break;
			case ScoreEvent.GoodSuccess:
				metrics.successfulGoodPackets++;
				metrics.totalGoodPackets++;
				break;
		}

		// Don't update score when a good packet reaches its destination
		if(_event == ScoreEvent.GoodSuccess) return;

		// Calculate the ratio of the maximum score the black hat should get
		float scoreRatio = (float)metrics.successfulMaliciousPackets / metrics.totalMaliciousPackets;
		// Calculate how the black hat's score should change
		float blackHatDerivative = scoreRatio * blackHatMaxScorePerEvent;
		// Calculate how the white hat's socre should change (taking into account destroy penalties)
		float whiteHatDerivative = (_event == ScoreEvent.GoodDestroyed ? -scorePenaltyForWhiteHatMistake : (1 - scoreRatio) * whiteHatMaxScorePerEvent);

		// Update the score
		blackHatScore += blackHatDerivative;
		whiteHatScore += whiteHatDerivative;

		// Propagate score update events
		scoreEvent?.Invoke(whiteHatDerivative, whiteHatScore, blackHatDerivative, blackHatScore);
		UpdateScoreUI();

		Debug.Log(metrics);
	}

	// Function which updates the score changes to the UI
	public void UpdateScoreUI(){
		BaseUI.instance.blackHatScoreText.text = "BlackHatScore: " + Mathf.RoundToInt(blackHatScore);
		BaseUI.instance.whiteHatScoreText.text = "WhiteHatScore: " + Mathf.RoundToInt(whiteHatScore);
	}
}
