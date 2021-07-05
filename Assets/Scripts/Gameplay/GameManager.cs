using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : Core.Utilities.SingletonPun<GameManager> {
	// Events called when a wave starts or ends
    public static Utilities.VoidEventCallback waveStartEvent;
	public static Utilities.VoidEventCallback waveEndEvent;
	// Event called when the game ends
	public static Utilities.VoidEventCallback gameEndEvent;


	// Property which returns true if the wave is currently started
	bool _waveStarted = false;
	public bool waveStarted {
		get => _waveStarted;
		protected set => _waveStarted = value;
	}

	// Reference to the ready button's text
	public TMPro.TextMeshProUGUI readyText;
	// Reference to the Game Over screen
	public GameObject endGameScreen;
	// References to the win and lose text in the game over screen
	public GameObject winText, loseText;

	// When we are dis/enabled register ourselves as a listener to playerPropertyUpdateEvents and roomOtherLeaveEvent
	void OnEnable(){
		NetworkingManager.roomPlayerPropertiesUpdateEvent += OnPlayerRoomPropertiesUpdate;
		NetworkingManager.roomOtherLeaveEvent += OnOtherPlayerLeave;
	}
	void OnDisable(){
		NetworkingManager.roomPlayerPropertiesUpdateEvent -= OnPlayerRoomPropertiesUpdate;
		NetworkingManager.roomOtherLeaveEvent -= OnOtherPlayerLeave;
	}

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

	// When the ready up button is pressed... ready up
	public void toggleReady(){
		setReady(!NetworkingManager.isReady);
	}

	// Whenever the disconnect button (or return to main menu button in the win screen) is pressed, leave the game and go back to the main menu
	public void OnDisconnectButtonPressed(){
		NetworkingManager.instance.LeaveRoomAndReturnToMainMenu();
	}

	// When the other play leaves the game, the player who remains wins the game
	public void OnOtherPlayerLeave(Player otherPlayer){
		EndGame(NetworkingManager.localPlayer);
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

	// Function which ends the game, marking which player won
	public void EndGame(Player winningPlayer) { if(NetworkingManager.isHost) photonView.RPC("RPC_GameManager_EndGame", RpcTarget.AllBuffered, winningPlayer.ActorNumber); }
	public void EndGame(int winningPlayerID) { if(NetworkingManager.isHost) photonView.RPC("RPC_GameManager_EndGame", RpcTarget.AllBuffered, winningPlayerID); }
	[PunRPC] void RPC_GameManager_EndGame(int winningPlayerID){
		endGameScreen.SetActive(true);
		if(winningPlayerID == NetworkingManager.localPlayer.ActorNumber)
			winText.SetActive(true);
		else loseText.SetActive(true);

		gameEndEvent?.Invoke();
	}
}
