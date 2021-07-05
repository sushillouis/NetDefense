using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : PathNodeBase {
	public static Destination[] destinations = null;

	// Whenever a new destination is added update the list of destinations
	protected override void Awake() {
		base.Awake();
		destinations = FindObjectsOfType<Destination>();
	}

	// All of the likelihoods are added together, then the probability of this point being a packet's destination is <likelihood>/<totalLikelihood>
	public int packetDestinationLikelihood = 1;

	// Function which returns a weighted list of starting points,
	public static Destination[] getWeightedList(){
		List<Destination> ret = new List<Destination>();
		// For each starting point, add it to the list a number of times equal to its <packetSourceLikelihood>
		foreach(Destination d in destinations)
			for(int i = 0; i < d.packetDestinationLikelihood; i++)
				ret.Add(d);

		return ret.ToArray();
	}
}
