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


	// Enum providing possible difficulty values
	[System.Serializable]
	public enum Difficulty {
		Easy = 0,
		Medium = 1,
		Hard = 2
	}
	// Static property describing the difficulty of the game
	static Difficulty _difficulty = Difficulty.Easy;
	public static Difficulty difficulty{
		get => _difficulty;
		set {
			// If an instance exists then network sync any changes
			if(instanceExists) instance.SetDifficulty(difficulty);
			// If an instance doesn't exist, then just hold the new value
			else _difficulty = difficulty;
		}
	}

	// Setting which determines the maximum number of waves before the game ends
	public int maximumWaves = 3;
	// Property determining the current wave
	[SerializeField] int _currentWave = 0;
	public int currentWave {
		get => _currentWave;
		protected set => _currentWave = value;
	}

	// Property which returns true if the wave is currently started
	bool _waveStarted = false;
	public bool waveStarted {
		get => _waveStarted;
		protected set => _waveStarted = value;
	}

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

		// When the game starts ensure that the difficulty level is synced with all of the players
		SetDifficulty(difficulty);

		if(NetworkingManager.instance) // Skip this step in debugging
		// Transfer control of the starting points and destinations to the BlackHatPlayer
		if(NetworkingManager.isHost){
			foreach(StartingPoint p in StartingPoint.startingPoints)
				p.photonView.TransferOwnership(NetworkingManager.blackHatPlayer);
			foreach(Destination d in Destination.destinations)
				d.photonView.TransferOwnership(NetworkingManager.blackHatPlayer);
		}
	}

	void Update(){
		// If the wave has been started, check to see if there are any packets still remaining
		if(waveStarted && !PacketEntityPoolManager.instance.packetsExist)
			EndWave();
	}

	// Helper function used to set the ready state even when debugging without a networking manager instance
	void setReady(bool isReady){
		// Play a sound to indicate that we are ready
		if(isReady) AudioManager.instance.uiSoundFXPlayer.PlayTrackImmediate("SettingsUpdated");

		if(NetworkingManager.instance)
			NetworkingManager.instance.setReady(isReady);
		else if(isReady) StartNextWave();
	}


	// -- Callbacks --


	// When player properties are updated, adjust the ready state and possibly start the game
	public void OnPlayerRoomPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable propertiesThatChanged){
		// Update the ready button text for this player
		BaseUI.instance.readyText.text = (NetworkingManager.isReady ? "Unready" : "Ready");

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
		// Determine the index of the winning player
		for(int i = 0; i < NetworkingManager.roomPlayers.Length; i++)
			if(NetworkingManager.localPlayer == NetworkingManager.roomPlayers[i]){
				EndGame(i);
				return;
			}

		// Or just guess that player 0 is the winning player if they can't be found
		EndGame(0); // TODO: is this assumption invalid?
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

		// Increase the current wave
		currentWave++;
		// If the current wave is greater than the maximum number of waves... end the game
		if(currentWave >= maximumWaves)
			EndGame(ScoreManager.instance.whiteHatScore >= ScoreManager.instance.blackHatScore ? NetworkingManager.whiteHatPlayerIndex : NetworkingManager.blackHatPlayerIndex);
	}

	// Function which ensures that all of the players are marked as unready (Network Synced)
	public void UnreadyAllPlayers() { photonView.RPC("RPC_GameManager_UnreadyAllPlayers", RpcTarget.AllBuffered); }
	[PunRPC] void RPC_GameManager_UnreadyAllPlayers(){
		// Mark the local player as not ready
		setReady(false);
	}

	// Function which ends the game, marking which player won (Network Synced)
	public void EndGame(int winningPlayerIndex) { if(NetworkingManager.isHost) photonView.RPC("RPC_GameManager_EndGame", RpcTarget.AllBuffered, winningPlayerIndex); }
	[PunRPC] void RPC_GameManager_EndGame(int winningPlayerIndex){
		try{
			// Show the win text if the player won
			if( (NetworkingManager.isSingleplayer && winningPlayerIndex == 0) // If the player in a single player game, won
				|| NetworkingManager.roomPlayers[winningPlayerIndex].ActorNumber == NetworkingManager.localPlayer.ActorNumber // If a player in a multiplayer game, won
			)
				BaseUI.instance.winText.SetActive(true);
			else BaseUI.instance.loseText.SetActive(true);
		// If we fail to find the index then assume that we lost
		} catch (System.IndexOutOfRangeException) { BaseUI.instance.loseText.SetActive(true); }

		// Show the game over screen
		BaseUI.instance.endGameScreen.SetActive(true);
		gameEndEvent?.Invoke();
	}

	// Function which sets the difficulty (Network Synced, ignores calls from players who aren't the host)
	public void SetDifficulty(Difficulty diff) { if(NetworkingManager.isHost) photonView.RPC("RPC_GameManager_SetDifficulty", RpcTarget.AllBuffered, diff); }
	[PunRPC] void RPC_GameManager_SetDifficulty(Difficulty diff) {
		_difficulty = diff;
	}
}
