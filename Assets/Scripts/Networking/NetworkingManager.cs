using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class NetworkingManager : Core.Utilities.PersistentSingletonPunCallbacks<NetworkingManager> {
	// Constant used to determine the hashtable key holding weather or not the host is a whitehat
	public const string IS_HOST_WHITE_HAT = "w";
	// Constant used to determine the hashtable key holding weather or not the player is readied up
	public const string IS_PLAYER_READY = "r";

	// Event callbacks
	public delegate void RoomListEventCallback(List<RoomInfo> roomList);
	public delegate void RoomPropertiesEventCallback(ExitGames.Client.Photon.Hashtable propertiesThatChanged);
	public delegate void RoomPlayerPropertiesEventCallback(Player targetPlayer, ExitGames.Client.Photon.Hashtable propertiesThatChanged);
	public delegate void OtherPlayerEventCallback(Player otherPlayer);
	// (Dis)connect
	public static Utilities.VoidEventCallback connectedEvent;
	public static Utilities.VoidEventCallback disconnectedEvent;
	// Join/Leave room
	public static Utilities.VoidEventCallback roomJoinEvent;
	public static Utilities.VoidEventCallback roomLeaveEvent;
	public static OtherPlayerEventCallback roomOtherJoinEvent;
	public static OtherPlayerEventCallback roomOtherLeaveEvent;
	// Room List
	public static RoomListEventCallback roomListUpdateEvent;
	// Properties
	public static RoomPropertiesEventCallback roomPropertiesUpdateEvent;
	public static RoomPlayerPropertiesEventCallback roomPlayerPropertiesUpdateEvent;

	// Variable used to track what happens when we disconnect
	enum DisconnectState {
		Simple,
		Reconnect,
		Offline,
		CreateOfflineRoom,
	}
	static DisconnectState disconnectState = DisconnectState.Simple;


	// Variables used when room creation fails and we need to try again
	int createRoomFailAttempts = 1;
	byte createPlayerCountCache;
	bool createIsWhiteHatCache;
	// Variable used to determine if we should return to the main menu when we leave a room or not
	static bool shouldReturnToMenuOnLeave = false;

	// The index (in PhotonNetwork.PlayerList) of the white and blackhat players
	public static int whiteHatPlayerIndex = -1, blackHatPlayerIndex = -1;



	// When enabled we setup the singleton and make sure we are connected to the PhotonNetwork
	public override void OnEnable(){
		base.OnEnable();

		// Ensure we are connected to Photon
		Reconnect();
	}

	// When disabled we cleanup the singleton and disconnect from the PhotonNetwork
	public override void OnDisable(){
		base.OnDisable();

		if(shouldReturnToMenuOnLeave) Reconnect();
		else PhotonNetwork.Disconnect();
	}


	// -- Networking Callbacks --


	// Once we connect to the network we should automatically join the lobby
	public override void OnConnectedToMaster(){
		Debug.Log("Connected to Photon (server " + PhotonNetwork.CloudRegion + ")");
		connectedEvent?.Invoke();

		PhotonNetwork.JoinLobby();
	}

	// When we disconnect from the network make sure to reset the player indices
	public override void OnDisconnected (DisconnectCause cause){
		Debug.Log("Disconnected from Photon beacuse " + cause + " with state " + disconnectState);

		whiteHatPlayerIndex = -1;
		blackHatPlayerIndex = -1;

		// If the disconnect was a part of a larger strategy then finish that action, otherwise fire the event
		switch(disconnectState){
			case DisconnectState.Simple: disconnectedEvent?.Invoke(); break;
			case DisconnectState.Reconnect: Reconnect(); break;
			case DisconnectState.Offline: GoOffline(); break;
			case DisconnectState.CreateOfflineRoom: CreateOfflineRoom(); break;
		}

		// Make sure that the disconnect state is reset
		disconnectState = DisconnectState.Simple;
		shouldReturnToMenuOnLeave = false;
	}

	// Pass along room updates to the connected listeners
	public override void OnRoomListUpdate (List<RoomInfo> roomList){ roomListUpdateEvent?.Invoke(roomList); }

	// If we failed to create a room (it is likely because the name was already taken) so add 1 to the room's name and try again
	public override void OnCreateRoomFailed (short returnCode, string message){
		Debug.Log("Failed to create room because: " + message);

		createRoomFailAttempts++;
		CreateRoom(PhotonNetwork.NickName + "'s room #" + createRoomFailAttempts, createPlayerCountCache, createIsWhiteHatCache);
	}

	// Pass room joins along to the attached listeners
	public override void OnJoinedRoom(){
		Debug.Log("Joined a room (" + PhotonNetwork.CurrentRoom.Name + ")");
		createRoomFailAttempts = 1;
		roomJoinEvent?.Invoke();
	}

	// When we leave a room, we might need to return to the main menu, if that is the case then do so
	public override void OnLeftRoom(){
		Debug.Log("Left room");

		whiteHatPlayerIndex = -1;
		blackHatPlayerIndex = -1;

		// Make sure we are still in the Lobby
		PhotonNetwork.JoinLobby();

		roomLeaveEvent?.Invoke();

		// Return to the main menu if requested
		if(shouldReturnToMenuOnLeave)
			SceneManager.LoadScene(0);
	}

	// Pass player joining room events along to the listeners
	public override void OnPlayerEnteredRoom (Player newPlayer){ roomOtherJoinEvent?.Invoke(newPlayer); }
	// Pass player leaving room events along to the listeners
	public override void OnPlayerLeftRoom (Player otherPlayer){ roomOtherLeaveEvent?.Invoke(otherPlayer); }

	// When the room properties update, we need to recalculate which player is the whitehat and which player is the blackhat
	public override void OnRoomPropertiesUpdate (ExitGames.Client.Photon.Hashtable propertiesThatChanged){
		// If the host whitehat property changed, figure out the index of the whitehat and blackhat players
		if(propertiesThatChanged[IS_HOST_WHITE_HAT] != null){
			// Determine the index in PhotonNetwork.PlayerList of the host and the connected secondary player (if there is no connected secondary player his index will be -1)
			int mainIndex = 0, secondaryIndex = -1;
			for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++){
				Player p = PhotonNetwork.PlayerList[i];
				if(p.ActorNumber == PhotonNetwork.CurrentRoom.MasterClientId)
					mainIndex = i;
				else secondaryIndex = i;
			}

			// If the host player is the whitehat, set the indices accordingly
			if((bool)propertiesThatChanged[IS_HOST_WHITE_HAT]){
				whiteHatPlayerIndex = mainIndex;
				blackHatPlayerIndex = secondaryIndex;
			// If the host player is the blackhat, set the indices accordingly
			} else {
				blackHatPlayerIndex = mainIndex;
				whiteHatPlayerIndex = secondaryIndex;
			}
		}

		// Propagate the event
		roomPropertiesUpdateEvent?.Invoke(propertiesThatChanged);
	}

	// Pass player property updates along to the listeners
	public override void OnPlayerPropertiesUpdate (Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps){ roomPlayerPropertiesUpdateEvent?.Invoke(targetPlayer, changedProps); }


	// -- Public Interaction Functions (Providing the Outside World a way to manipulate the network)


	// Creates a new room (and joins it) with the specified <name>, capable of hosting the provided <playerCount>, and with the set hostWhitehat status
	public void CreateRoom(string name, byte playerCount, bool isHostWhitehat){
		RoomOptions options = new RoomOptions(){
			MaxPlayers = playerCount,
			CustomRoomProperties = new ExitGames.Client.Photon.Hashtable(){
				{IS_HOST_WHITE_HAT, isHostWhitehat} // hat
			}
		};

		PhotonNetwork.CreateRoom(name, options);
		createIsWhiteHatCache = isHostWhitehat;
		createPlayerCountCache = playerCount;
	}
	// Creates a new room same as above, but its name is based on the player's name
	public void CreateRoom(byte playerCount, bool isHostWhitehat){ CreateRoom(PhotonNetwork.NickName + "'s room", playerCount, isHostWhitehat); }

	// Join the specified room
	public void JoinRoom(string name){ PhotonNetwork.JoinRoom(name); }

	// Leave the current room
	public void LeaveRoom(){
		// Don't bother if we aren't in a room
		if(PhotonNetwork.CurrentRoom == null) return;

		// Make sure that we aren't marked as ready
		PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable(){
			{NetworkingManager.IS_PLAYER_READY, false}
		});

		// Leave the room
		PhotonNetwork.LeaveRoom();
	}
	// Leave the current room (and also return to the main menu)
	public void LeaveRoomAndReturnToMainMenu(){
		shouldReturnToMenuOnLeave = true;
		LeaveRoom();
	}

	// Set the properties (Most relevant being the white hat status) of the room
	public void SetRoomProperties(ExitGames.Client.Photon.Hashtable changed){
		// Don't bother if we aren't in a room
		if(PhotonNetwork.CurrentRoom == null) return;

		PhotonNetwork.CurrentRoom.SetCustomProperties(changed);
	}

	// Disconnect then reconnect to the PhotonNetwork
	public void Reconnect(){
		// If we are connected... first disconnect
		if(PhotonNetwork.IsConnected){
			disconnectState = DisconnectState.Reconnect;
			PhotonNetwork.Disconnect();
			return;
		}

		// Connect to the photon network
		PhotonNetwork.ConnectUsingSettings();
		// Make sure that scene transitions are done automatically
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	// Disconnect from the PhotonNetwork then go into offline mode
	public void GoOffline(){
		// If we are connected... first disconnect
		if(PhotonNetwork.IsConnected){
			disconnectState = DisconnectState.Offline;
			PhotonNetwork.Disconnect();
			return;
		}

		// Switch into offline mode
		PhotonNetwork.OfflineMode = true;
	}

	// Disconnect from the PhotonNetwork, go into offline mode, and create an offline room
	public void CreateOfflineRoom(){
		// Switch into offline mode
		GoOffline();
		if(PhotonNetwork.IsConnected) disconnectState = DisconnectState.CreateOfflineRoom;

		// Join a random room while in offline mode
		PhotonNetwork.JoinRandomRoom();
	}


	// -- Ready Functions


	// Check if the provided player has readied up
	public bool isPlayerReady(Player p){
		if(p.CustomProperties[IS_PLAYER_READY] == null) return false;

		return (bool)p.CustomProperties[IS_PLAYER_READY];
	}
	// Check if the provided player (fetched based on its index in the player list array) is readied up
	public bool isPlayerReady(int index){ return isPlayerReady(PhotonNetwork.PlayerList[index]); }

	// Check if all players have readied up
	public bool allPlayersReady(){
		foreach(Player p in PhotonNetwork.PlayerList)
			if(!isPlayerReady(p)) return false;

		return true;
	}

	// Marks the local player as being ready
	public void setReady(bool isReady){
		localPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable(){
			{IS_PLAYER_READY, isReady}
		});
	}

	// Toggles weather or not the local player is ready.
	public void toggleReady(){ setReady( !isPlayerReady(localPlayer) ); }


	// -- Properties --


	// Returns true if we are in a singleplayer session
	public static bool isSingleplayer {
		get => PhotonNetwork.OfflineMode == true;
	}

	// Returns true if we are the multiplayer session's host or if we are in a singleplayer session
	public static bool isHost {
		get {
			if(isSingleplayer) return true;
			return PhotonNetwork.IsMasterClient;
		}
	}

	// Returns true if we are the whitehat player
	public static bool isWhiteHat {
		get {
			if(whiteHatPlayerIndex == -1) return false;
			if(isSingleplayer) return whiteHatPlayerIndex == 0;
			return PhotonNetwork.PlayerList[whiteHatPlayerIndex].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		}
	}

	// Returns true if we are the blackhat player
	public static bool isBlackHat {
		get {
			if(blackHatPlayerIndex == -1) return false;
			if(isSingleplayer) return blackHatPlayerIndex == 0;
			return PhotonNetwork.PlayerList[blackHatPlayerIndex].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		}
	}

	// Returns true if we are currently in a room
	public static bool inRoom {
		get => PhotonNetwork.CurrentRoom != null;
	}

	// Returns true if the room is currently at its maximum capacity
	public static bool isRoomFull {
		get => PhotonNetwork.CurrentRoom.MaxPlayers == NetworkingManager.roomPlayers.Length;
	}

	// Pass along the local player, so that the rest of the game only has to interact with us (and not photon)
	public static Player localPlayer {
		get => PhotonNetwork.LocalPlayer;
	}

	// Returns true if the local player is marked as ready
	public static bool isReady {
		get {
			if(localPlayer.CustomProperties[IS_PLAYER_READY] == null) return false;
			return (bool)localPlayer.CustomProperties[IS_PLAYER_READY];
		}
	}

	// Pass along the list of players currently in the room, so that the rest of the game only has to interact with us (and not photon)
	public static Player[] roomPlayers {
		get => PhotonNetwork.PlayerList;
	}

	public static Player whiteHatPlayer {
		get {
			if(whiteHatPlayerIndex < 0) return null;
			return roomPlayers[whiteHatPlayerIndex];
		}
	}

	public static Player blackHatPlayer {
		get {
			if(blackHatPlayerIndex < 0) return null;
			return roomPlayers[blackHatPlayerIndex];
		}
	}
}
