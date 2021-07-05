using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PacketEntityPoolManager : Core.Utilities.Singleton<PacketEntityPoolManager> {

	// Path to the packet prefab we should spawn
	public string packetPrefabPath;

	// The number of seconds that should elapse before another packet spawns
	public float secondsBetweenPackets = 1.5f;

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
		// Generate the weighted lists used to determine the start and end points
		StartingPoint[] startingPoints = StartingPoint.getWeightedList();
		Destination[] destinations = Destination.getWeightedList();

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

				// Pick a random starting point and destination for it (network synced)
				spawned.setStartDestinationAndPath(startingPoints[Random.Range(0, startingPoints.Length)], destinations[Random.Range(0, destinations.Length)]);
				spawned.transform.rotation = spawned.startPoint.transform.rotation; // Ensure that the packets have the same orientation as their spawners

				// Setup the packet's appearance and malicious status (network synced)
				spawned.initPacketDetails();

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
