using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// Script which spawns the appropriate black hat or white hat interfaces and managers
public class SideSpawner : MonoBehaviour {
	// Prefabs to spawn
	public GameObject whiteHatPrefab, blackHatPrefab;

	// When the scene starts spawn the correct side
	void Awake(){
		if(NetworkingManager.isBlackHat) Instantiate(blackHatPrefab);
		else Instantiate(whiteHatPrefab);
		
		Destroy(gameObject);
	}
}
