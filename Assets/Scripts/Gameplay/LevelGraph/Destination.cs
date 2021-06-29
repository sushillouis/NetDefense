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
}
