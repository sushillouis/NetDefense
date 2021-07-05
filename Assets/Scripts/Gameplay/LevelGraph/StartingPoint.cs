using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingPoint : PathNodeBase {
	public static StartingPoint[] startingPoints = null;

	// Whenever a new starting point is added update the list of starting points
	protected override void Awake() {
		base.Awake();
		startingPoints = FindObjectsOfType<StartingPoint>();
	}

	// All of the likelihoods are added together, then the probability of spawning a packet at this point is <likelihood>/<totalLikelihood>
	public int packetSourceLikelihood = 1;
	// The malicious packet for this starting point
	public Packet.Details maliciousPacketDetails;
	// The likelihood that a packet coming from this starting point will be malicious
	public float maliciousPacketProbability = .33333f;

	// Generates a random set of details, ensuring that the returned values aren't considered malicious
	public Packet.Details randomNonMaliciousDetails() {
		Packet.Details details = new Packet.Details(Utilities.randomEnum<Packet.Color>(), Utilities.randomEnum<Packet.Size>(), Utilities.randomEnum<Packet.Shape>());
		if(details == maliciousPacketDetails) details = randomNonMaliciousDetails();
		return details;
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
