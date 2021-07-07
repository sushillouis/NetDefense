using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TestBench : MonoBehaviour {
	public PathNodeBase target;

	void Awake(){
		if(!PhotonNetwork.IsConnected){
			PhotonNetwork.OfflineMode = true;
			PhotonNetwork.JoinRandomRoom();
		}
	}

    // Start is called before the first frame update
    void Start() {
		// Mute the musicPlayer
		(AudioManager.instance as AudioManager).musicPlayer.volume = 0;

        PathNodeBase pnb = GetComponent<PathNodeBase>();

		var path = pnb.findPathTo(target);

		string debug = "";
		foreach(var c in path)
			debug += c + ", ";
		Debug.Log(debug);


    }

}
