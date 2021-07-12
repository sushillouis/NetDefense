using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Destination : PathNodeBase, SelectionManager.ISelectable {
	public static Destination[] destinations = null;

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

	// Whenever a new destination is added update the list of destinations
	protected override void Awake() {
		base.Awake();
		destinations = FindObjectsOfType<Destination>();
	}

	// The number of updates gained after each wave (based on difficulty)
	public int[] updatesGrantedPerWave = new int[3] {/*easy*/2, /*medium*/2, /*hard*/2};
	// Property representing the number of updates currently available
	public int updatesRemaining = 0;

	// All of the likelihoods are added together, then the probability of this point being a packet's destination is <likelihood>/<totalLikelihood>
	public int packetDestinationLikelihood = 1;
	// Likelihood for malicious packets to target this destination
	[SerializeField] int _maliciousPacketDestinationLikelihood = 1;
	public int maliciousPacketDestinationLikelihood {
		get => _maliciousPacketDestinationLikelihood;
		set => SetMaliciousPacketDestinationLikelihood(value);
	}


	// De/register the start function on wave ends
	void OnEnable(){ GameManager.waveEndEvent += Start; }
	void OnDisable(){ GameManager.waveEndEvent -= Start; }

	// When the this is created or a wave starts grant its updates per wave
	void Start(){
		updatesRemaining += updatesGrantedPerWave[(int)GameManager.difficulty];
	}

	// Function which updates the likelihood of a malicious packet targeting this destination (Network Synced)
	public bool SetMaliciousPacketDestinationLikelihood(int likelihood) {
		// Only update the settings if we have updates remaining
		if(updatesRemaining > 0){
			// Take away an update if something actually changed
			if(packetDestinationLikelihood != likelihood)
				updatesRemaining--;
			photonView.RPC("RPC_Destination_SetMaliciousPacketDestinationLikelihood", RpcTarget.AllBuffered, likelihood);
		} else return false;
		return true;
	}
	[PunRPC] void RPC_Destination_SetMaliciousPacketDestinationLikelihood(int likelihood){
		_maliciousPacketDestinationLikelihood = likelihood;
	}

	// Function which returns a weighted list of destinations
	public static Destination[] getWeightedList(){
		List<Destination> ret = new List<Destination>();
		// For each starting point, add it to the list a number of times equal to its <packetSourceLikelihood>
		foreach(Destination d in destinations)
			for(int i = 0; i < d.packetDestinationLikelihood; i++)
				ret.Add(d);

		return ret.ToArray();
	}

	// Function which returns a weighted list of destinations for malicious packets
	public static Destination[] getMaliciousWeightedList(){
		List<Destination> ret = new List<Destination>();
		// For each starting point, add it to the list a number of times equal to its <packetSourceLikelihood>
		foreach(Destination d in destinations)
			for(int i = 0; i < d.maliciousPacketDestinationLikelihood; i++)
				ret.Add(d);

		return ret.ToArray();
	}
}
