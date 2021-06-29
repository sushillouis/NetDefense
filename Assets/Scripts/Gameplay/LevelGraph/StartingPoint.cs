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
}
