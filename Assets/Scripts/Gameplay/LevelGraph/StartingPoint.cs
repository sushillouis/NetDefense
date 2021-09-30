using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StartingPoint : PathNodeBase, SelectionManager.ISelectable {
	public static StartingPoint[] startingPoints = null;

	// Cache of the attached photon view
	private PhotonView pvCache;
	public PhotonView photonView
	{
		get
		{
			#if UNITY_EDITOR
			// In the editor we want to avoid caching this at design time, so changes in PV structure appear immediately.
			if (!Application.isPlaying || this.pvCache == null)
			{
				this.pvCache = PhotonView.Get(this);
			}
			#else
			if (this.pvCache == null)
			{
				this.pvCache = PhotonView.Get(this);
			}
			#endif
			return this.pvCache;
		}
	}

	// Whenever a new starting point is added update the list of starting points
	protected override void Awake() {
		base.Awake();
		startingPoints = FindObjectsOfType<StartingPoint>();
	}

	// The number of updates gained after each wave (based on difficulty)
	public int[] updatesGrantedPerWave = new int[3] {/*easy*/5, /*medium*/5, /*hard*/5};
	// Property representing the number of updates currently available
	public int updatesRemaining = 1; // Starts at 1 to account for initial settings

	// All of the likelihoods are added together, then the probability of spawning a packet at this point is <likelihood>/<totalLikelihood>
	public int packetSourceLikelihood = 0;


	// De/register the start function on wave ends
	void OnEnable(){ GameManager.waveEndEvent += Start; }
	void OnDisable(){ GameManager.waveEndEvent -= Start; }

	// When the this is created or a wave starts grant its updates per wave
	void Start(){
		updatesRemaining += updatesGrantedPerWave[(int)GameManager.difficulty];
	}

	// The malicious packet for this starting point (NetworkSynced)
	public Packet.Details spawnedMaliciousPacketDetails = Packet.Details.Default;
	// The likelihood that a packet coming from this starting point will be malicious (Network Synced)
	public float maliciousPacketProbability = .33333f;

	// Generates a random set of details, ensuring that the returned values aren't considered malicious
	public Packet.Details randomNonMaliciousDetails() {
		Packet.Details details = new Packet.Details(Utilities.randomEnum<Packet.Color>(), Utilities.randomEnum<Packet.Size>(), Utilities.randomEnum<Packet.Shape>());
		if(details == spawnedMaliciousPacketDetails) details = randomNonMaliciousDetails();
		return details;
	}


	// Update the starting point's malicious packet details (Network Synced)
	// Returns true if we successfully updated, returns false otherwise
	public bool SetMaliciousPacketDetails(Packet.Color color, Packet.Size size, Packet.Shape shape) {
		// Only update the settings if we have updates remaining
		if(updatesRemaining > 0){
			// Take away an update if something actually changed
			if(color != spawnedMaliciousPacketDetails.color || size != spawnedMaliciousPacketDetails.size || shape != spawnedMaliciousPacketDetails.shape)
				updatesRemaining--;
			photonView.RPC("RPC_StartingPoint_SetspawnedMaliciousPacketDetails", RpcTarget.AllBuffered, color, size, shape);
			return true;
		} else return false;
	}
	public bool SetMaliciousPacketDetails(Packet.Details details) { return SetMaliciousPacketDetails(details.color, details.size, details.shape); }
	[PunRPC] void RPC_StartingPoint_SetMaliciousPacketDetails(Packet.Color color, Packet.Size size, Packet.Shape shape){
		spawnedMaliciousPacketDetails = new Packet.Details(color, size, shape);
	}


	// Function which updates the probability of a spawned packet being malicious (Network Synced)
	// Returns true if we successfully updated, returns false otherwise
	public bool SetMaliciousPacketProbability(float probability) {
		// Only update the settings if we have updates remaining
		if(updatesRemaining > 0){
			// Take away an update if something actually changed
			if(maliciousPacketProbability != probability)
				updatesRemaining--;
			photonView.RPC("RPC_StartingPoint_SetMaliciousPacketProbability", RpcTarget.AllBuffered, probability);
			return true;
		} else return false;
	}
	[PunRPC] void RPC_StartingPoint_SetMaliciousPacketProbability(float probability){
		maliciousPacketProbability = probability;
	}

	// Function which returns a weighted list of starting points,
	public static StartingPoint[] getWeightedList(){
		List<StartingPoint> ret = new List<StartingPoint>();
		// For each starting point, add it to the list a number of times equal to its <packetSourceLikelihood>
		foreach(StartingPoint p in startingPoints)
			for(int i = 0; i < p.packetSourceLikelihood; i++)
				ret.Add(p);

		return ret.ToArray();
	}
}
