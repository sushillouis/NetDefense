using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : Core.Utilities.SingletonPun<GameManager> {
	// Events called when a wave starts or ends
    public static Utilities.VoidEventCallback waveStartEvent;
	public static Utilities.VoidEventCallback waveEndEvent;

	// Property determining if the wave is currently started
	bool _waveStarted = false;
	public bool waveStarted {
		get => _waveStarted;
		protected set => _waveStarted = value;
	}

	// Reference to the ready button's text
	public TMPro.TextMeshProUGUI readyText;


	// When we are dis/enabled register ourselves as a listener to playerPropertyUpdateEvents
	void OnEnable(){ NetworkingManager.roomPlayerPropertiesUpdateEvent += OnPlayerRoomPropertiesUpdate; }
	void OnDisable(){ NetworkingManager.roomPlayerPropertiesUpdateEvent -= OnPlayerRoomPropertiesUpdate; }

	void Start(){
		// When we load into the scene make sure that every player is not readied
		if(NetworkingManager.instance) NetworkingManager.instance.setReady(false);
	}

	void Update(){
		// If the wave has been started, check to see if there are any packets still remaining
		if(waveStarted && !PacketEntityPoolManager.instance.packetsExist)
			EndWave();
	}

	// Helper function used to set the ready state even when debugging without a networking manager instance
	void setReady(bool isReady){
		if(NetworkingManager.instance)
			NetworkingManager.instance.setReady(isReady);
		else if(isReady) StartNextWave();
	}


	// -- Callbacks --


	// When player properties are updated, adjust the ready state and possibly start the game
	public void OnPlayerRoomPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable propertiesThatChanged){
		// Update the ready button text for this player
		readyText.text = (NetworkingManager.isReady ? "Unready" : "Ready");

		// If all of the players are ready, then start the next wave (Network Synced, Function ignores calls from everyone but host)
		if(NetworkingManager.instance.allPlayersReady())
			StartNextWave();
	}

	// Function called when the ready up button is pressed
	public void toggleReady(){
		setReady(!NetworkingManager.isReady);
	}


	// -- Network Sync --


	// Function which starts the next wave (Network Synced, ignores calls from players who aren't the host)
	public void StartNextWave() { if(NetworkingManager.isHost) photonView.RPC("RPC_GameManager_StartNextWave", RpcTarget.AllBuffered); }
	[PunRPC] void RPC_GameManager_StartNextWave(){
		Debug.Log("Starting Next Wave!");

		// Ensure that once the wave starts, all of the players are marked as unready
		if(NetworkingManager.isHost) UnreadyAllPlayers();

		waveStartEvent?.Invoke();
		waveStarted = true; // Mark that the wave has started
	}

	// Function which ends the current wave (Network Synced, ignores calls from players who aren't the host)
	public void EndWave() { if(NetworkingManager.isHost) photonView.RPC("RPC_GameManager_EndWave", RpcTarget.AllBuffered); }
	[PunRPC] void RPC_GameManager_EndWave(){
		Debug.Log("Wave Ended!");

		waveEndEvent?.Invoke();
		waveStarted = false; // mark that the wave has ended
	}

	// Function which ensures that all of the players are marked as unready
	public void UnreadyAllPlayers() { photonView.RPC("RPC_GameManager_UnreadyAllPlayers", RpcTarget.AllBuffered); }
	[PunRPC] void RPC_GameManager_UnreadyAllPlayers(){
		// Mark the local player as not ready
		setReady(false);
	}


}
