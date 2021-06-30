using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PacketPoolManager : Core.Utilities.Singleton<PacketPoolManager> {

	// Path to the packet prefab we should spawn
	public string packetPrefabPath;

	// The number of seconds that should elapse before another packet spawns
	public float secondsBetweenPackets = 1.5f;
	// The probability of a packet being malicious
	public float maliciousProbability = .33333f;

	// Property tracking weather or not we are currently spawning packets
	bool _isSpawning = false;
	public bool isSpawning {
		get => _isSpawning;
		protected set { _isSpawning = value; }
	}

	// Ensure that the packet path is valid if it was coppied from unity
	protected override void Awake(){
		base.Awake();
		Utilities.PreparePrefabPath(ref packetPrefabPath);
	}

	void Start(){
		StartSpawningPackets(30);
	}

	// Function which begins spawning the specified number of packets
	public void StartSpawningPackets(int toSpawn){ StartCoroutine(SpawnPackets(toSpawn)); }
	// Coroutine which spawns the specified number of packets
	IEnumerator SpawnPackets(int toSpawn){
		Debug.Log(NetworkingManager.isHost);
		// Packet spawning can only be preformed by the host
		if(NetworkingManager.isHost){
			// Mark that we are spawning
			isSpawning = true;

			// For each packet we should spawn...
			for(int i = 0; i < toSpawn; i++){
				// Spawn the packet over the network
				Packet spawned = PhotonNetwork.InstantiateRoomObject(packetPrefabPath, new Vector3(0, 100, 0), Quaternion.identity).GetComponent<Packet>();
				// Locally parent it to ourselves
				spawned.transform.parent = transform;

				// Determine if is malicious or not (network synced)
				spawned.isMalicious = Random.Range(0f, 1f) <= maliciousProbability;
				// If it isn't malicious, determine what it looks like (network synced)
				if(!spawned.isMalicious) spawned.details = Packet.Details.randomNonMaliciousDetails();

				// Pick a random starting point amd destination for it (network synced)
				spawned.setStartDestinationAndPath(StartingPoint.startingPoints[Random.Range(0, StartingPoint.startingPoints.Length)], Destination.destinations[Random.Range(0, Destination.destinations.Length)]);

				// Wait for the configured time between packets
				yield return new WaitForSeconds(secondsBetweenPackets);
			}
		}

		// Mark that we have finished spawning
		isSpawning = false;
	}




	public bool hasChildren {
		get => transform.childCount > 0;
	}
}
