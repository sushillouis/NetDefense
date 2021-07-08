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

	// All of the likelihoods are added together, then the probability of spawning a packet at this point is <likelihood>/<totalLikelihood>
	public int packetSourceLikelihood = 1;

	// The malicious packet for this starting point (NetworkSynced)
	[SerializeField] Packet.Details _maliciousPacketDetails = Packet.Details.Default;
	public Packet.Details maliciousPacketDetails {
		get => _maliciousPacketDetails;
		set => SetMaliciousPacketDetails(value);
	}
	// The likelihood that a packet coming from this starting point will be malicious (Network Synced)
	[SerializeField] float _maliciousPacketProbability = .33333f;
	public float maliciousPacketProbability {
		get => _maliciousPacketProbability;
		set => SetMaliciousPacketProbability(value);
	}

	// Generates a random set of details, ensuring that the returned values aren't considered malicious
	public Packet.Details randomNonMaliciousDetails() {
		Packet.Details details = new Packet.Details(Utilities.randomEnum<Packet.Color>(), Utilities.randomEnum<Packet.Size>(), Utilities.randomEnum<Packet.Shape>());
		if(details == maliciousPacketDetails) details = randomNonMaliciousDetails();
		return details;
	}


	// Update the starting point's malicious packet details (Network Synced)
	public void SetMaliciousPacketDetails(Packet.Color color, Packet.Size size, Packet.Shape shape) { photonView.RPC("RPC_StartingPoint_SetMaliciousPacketDetails", RpcTarget.AllBuffered, color, size, shape); }
	public void SetMaliciousPacketDetails(Packet.Details details) { photonView.RPC("RPC_StartingPoint_SetMaliciousPacketDetails", RpcTarget.AllBuffered, details.color, details.size, details.shape); }
	[PunRPC] void RPC_StartingPoint_SetMaliciousPacketDetails(Packet.Color color, Packet.Size size, Packet.Shape shape){
		_maliciousPacketDetails = new Packet.Details(color, size, shape);
	}


	// Function which updates the probability of a spawned packet being malicious (Network Synced)
	public void SetMaliciousPacketProbability(float probability) { photonView.RPC("RPC_StartingPoint_SetMaliciousPacketProbability", RpcTarget.AllBuffered, probability); }
	[PunRPC] void RPC_StartingPoint_SetMaliciousPacketProbability(float probability){
		_maliciousPacketProbability = probability;
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
