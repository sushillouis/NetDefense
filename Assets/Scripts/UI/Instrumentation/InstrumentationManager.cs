using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentationManager : Core.Utilities.Singleton<InstrumentationManager> {
	[System.Serializable]
	public struct InstrumentationEvent {
		public int playerID;
		public float timestamp;
		public string source;
		public string eventType;
		public string data;
	}

	[SerializeField]
	List<InstrumentationEvent> eventLog = new List<InstrumentationEvent>();

	// Generates a new event, already referencing the local player with the correct current timestamp
	public InstrumentationEvent generateNewEvent(){
		InstrumentationEvent e = new InstrumentationEvent();
		// Player ID won't be found while debugging... so return 1 in that case
		try { e.playerID = NetworkingManager.localPlayer.actorNumber;
		} catch (System.NullReferenceException){ e.playerID = 1; }
		e.timestamp = Time.time;
		return e;
	}

	public void LogInstrumentationEvent(InstrumentationEvent e) { eventLog.Add(e); }
}
