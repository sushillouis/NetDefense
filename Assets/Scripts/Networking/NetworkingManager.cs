using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class NetworkingManager : Core.Utilities.SingletonPunCallbacks<NetworkingManager> {
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
	public static Utilities.VoidEventCallback becomeRoomHostEvent;
	// Room List
	public static RoomListEventCallback roomListUpdateEvent;
	// Properties
	public static RoomPropertiesEventCallback roomPropertiesUpdateEvent;
	public static RoomPlayerPropertiesEventCallback roomPlayerPropertiesUpdateEvent;
	public static Utilities.VoidEventCallback roomStateUpdateEvent;


	// Reference to the PhotonView attached to this object
	private PhotonView pvCache;
	public PhotonView photonView {
		get {
#if UNITY_EDITOR
			// In the editor we want to avoid caching this at design time, so changes in PV structure appear immediately.
			if (!Application.isPlaying || this.pvCache is null)
				this.pvCache = PhotonView.Get(this);
#else
			if (this.pvCache is null)
				this.pvCache = PhotonView.Get(this);
#endif
			return this.pvCache;
		}
	}


	// Variable used to track what happens when we disconnect
	enum DisconnectState {
		Simple,
		Reconnect,
		GoOffline,
		CreateOfflineRoom,
	}
	static DisconnectState disconnectState = DisconnectState.Simple;


	// Variables used when room creation fails and we need to try again
	int createRoomFailAttempts = 1;
	byte createPlayerCountCache;
	bool createIsWhiteHatCache;
	// Variable used to determine if we should return to the main menu when we leave a room or not
	static bool shouldReturnToMenuOnLeave = false;
	// Variable which tracks if we are localy considering ourselves the room host, used to fire
	static bool isRoomHost = false;

	// The index (in PhotonNetwork.PlayerList) of the white and blackhat players
	public static List<Networking.Player> players = null;
	public List<Networking.Player> debuggingPlayers = null; // TODO: Remove


	// Register the de/serialization functions for Networking.Player
	protected override void Awake(){
		base.Awake();

		ExitGames.Client.Photon.PhotonPeer.RegisterType(typeof(Networking.Player), /*Can't be registerd as P or Q since they are reserved by photon*/(byte)'P' + 2, Networking.Player.PhotonSerialize, Networking.Player.PhotonDeserialize);
	}

	// When enabled we setup the singleton and make sure we are connected to the PhotonNetwork
	public override void OnEnable(){
		base.OnEnable();

		// If we are already connected... don't bother connecting
		if(PhotonNetwork.IsConnected) return;

		// Ensure we are connected to Photon
		Reconnect();
	}

	// When disabled we cleanup the singleton and disconnect from the PhotonNetwork
	public override void OnDisable(){
		base.OnDisable();

		// Don't bother disconnecting from photon if we are in a room
		if(inRoom) return;

		// If we are leaving a game and returning to the main menu then we should reconnect to photon
		if(shouldReturnToMenuOnLeave) Reconnect();
		// If we are exiting the game then disconnect from photon
		else PhotonNetwork.Disconnect();
	}


	// -- Networking Callbacks --


	// Once we connect to the network we should automatically join the lobby
	public override void OnConnectedToMaster(){
		Debug.Log("Connected to Photon (server " + PhotonNetwork.CloudRegion + ")");
		connectedEvent?.Invoke();

		PhotonNetwork.JoinLobby();
	}

	// When we disconnect from the newtwork, do something based on our disconnect state
	public override void OnDisconnected (DisconnectCause cause){
		Debug.Log("Disconnected from Photon because " + cause + " with state " + disconnectState);
		// When we disconnect we are no longer the room host
		isRoomHost = false;

		// If the disconnect was a part of a larger strategy then finish that action, otherwise fire the event
		switch(disconnectState){
			case DisconnectState.Simple: disconnectedEvent?.Invoke(); break; 		// Simple means that we disconnect (and trigger the associated event)
			case DisconnectState.Reconnect: Reconnect(); break;						// Reconnect means that we should immediately reconnect to the network
			case DisconnectState.GoOffline: GoOffline(); break;						// GoOffline means that we should immediately reconnect in offline mode
			case DisconnectState.CreateOfflineRoom: CreateOfflineRoom(); break;		// CreateOfflineRoom means that we should immediately reconnect in offline mode and create a new room
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

	// When we join a room, create a new player list if we are the first player
	public override void OnJoinedRoom(){
		Debug.Log("Joined a room (" + PhotonNetwork.CurrentRoom.Name + ")");
		createRoomFailAttempts = 1;

		// If we are the host or this is a singleplayer game...
		if(isHost || isSingleplayer){
			// Create a new list to hold the room's players
			debuggingPlayers = players = new List<Networking.Player>();
			// Create a Networking.Player representing ourselves
			Networking.Player me = new Networking.Player();
			me.photonPlayer = PhotonNetwork.LocalPlayer;
			me.debugPhotonPlayer = me.photonPlayer.ActorNumber; // TODO: Remove
			// Add it to the list
			players.Add(me);

			// Setup the local player
			RPC_NetworkingManager_RoomStateSynchronization(players.ToArray(), null);
			// Trigger the join room event
			roomJoinEvent?.Invoke();

			// Mark ourselves as being the room host and trigger the become host event
			isRoomHost = true;
			becomeRoomHostEvent?.Invoke();
		} else
			// Request room state synchronization, marking sure that we should trigger the join room event once synchronization is complete
			RequestRoomStateSynchronization(PhotonNetwork.LocalPlayer);
	}

	// When we leave a room, we might need to return to the main menu, if that is the case then do so
	public override void OnLeftRoom(){
		Debug.Log("Left room");

		// Clear the list of players and local player
		debuggingPlayers = players = null;
		Networking.Player.localPlayer = null;
		// If we aren't in a room we definitely aren't the host
		isRoomHost = false;

		// Make sure we are still in the Lobby
		PhotonNetwork.JoinLobby();

		// Propagate the event
		roomLeaveEvent?.Invoke();

		// Return to the main menu if requested
		if(shouldReturnToMenuOnLeave)
			SceneManager.LoadScene(0);
	}

	// Pass player joining room events along to the listeners, and update the current list of players
	public override void OnPlayerEnteredRoom (Player newPlayer){
		// If we are the host...
		if(isHost){
			// Create a Networking.Player for the newly joined player and add it to the player list
			Networking.Player _new = new Networking.Player();
			_new.photonPlayer = newPlayer;
			_new.debugPhotonPlayer = _new.photonPlayer.ActorNumber; // TODO: Remove
			players.Add(_new);

			// Synchronize our list of players with the rest of the lobby
			RequestRoomStateSynchronization();
		}

		// Propagate the event
		roomOtherJoinEvent?.Invoke(newPlayer);
	}

	// Pass player leaving room events along to the listeners, and update the current list of players
	public override void OnPlayerLeftRoom (Player otherPlayer){
		// If we are the host...
		if(isHost){
			// If we weren't already the host, trigger the become host event
			if(!isRoomHost){
				isRoomHost = true;
				becomeRoomHostEvent?.Invoke();
			}

			// Remove the player who left from our list of players
			var removeIndex = players.FindIndex(p => p.photonPlayer == otherPlayer);
			players.RemoveAt(removeIndex);

			// Synchronize our list of players with the rest of the lobby
			RequestRoomStateSynchronization();
		}

		// Propagate the event
		roomOtherLeaveEvent?.Invoke(otherPlayer);
	}

	// When the room properties update, propagate the event
	public override void OnRoomPropertiesUpdate (ExitGames.Client.Photon.Hashtable propertiesThatChanged){ roomPropertiesUpdateEvent?.Invoke(propertiesThatChanged); }

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
		if(PhotonNetwork.CurrentRoom is null) return;

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
		if(PhotonNetwork.CurrentRoom is null) return;

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
			disconnectState = DisconnectState.GoOffline;
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


	// -- Room State Functions --


	// Function which causes the current player to become a whitehat
	public void BecomeWhiteHat(){ photonView.RPC("RPC_NetworkingManager_BecomeWhiteHat", RpcTarget.AllBuffered, localPlayer.actorNumber); }
	[PunRPC] void RPC_NetworkingManager_BecomeWhiteHat(int playerActorNumber){
		// Room state management can only be preformed by the host
		if(!isHost) return;

		// Reference to the current primary player
		var whiteHatPrimaryPlayer = NetworkingManager.whiteHatPrimaryPlayer;
		// Find the player who requested to become a whitehat
		Networking.Player player = players.Find(p => p.actorNumber == playerActorNumber);

		// Make the requesting player a whitehat
		player.side = Networking.Player.Side.WhiteHat;

		// If the whitehats already have a primary player... then the requesting player becomes an advisor
		if(whiteHatPrimaryPlayer is object)
			player.role = Networking.Player.Role.Advisor;
		// Otherwise... the requesting player becomes their primary player
		else if(player.role == Networking.Player.Role.Spectator)
			player.role = Networking.Player.Role.Player;

		// Synchronize the room state
		RequestRoomStateSynchronization();
	}

	// Function which causes the current player to become a blackhat
	public void BecomeBlackHat(){ photonView.RPC("RPC_NetworkingManager_BecomeBlackHat", RpcTarget.AllBuffered, localPlayer.actorNumber); }
	[PunRPC] void RPC_NetworkingManager_BecomeBlackHat(int playerActorNumber){
		// Room state management can only be preformed by the host
		if(!isHost) return;

		// Reference to the current primary player
		var blackHatPrimaryPlayer = NetworkingManager.blackHatPrimaryPlayer;
		// Find the player who requested to become a blackhat
		Networking.Player player = players.Find(p => p.actorNumber == playerActorNumber);

		// Make the requesting player a blackhat
		player.side = Networking.Player.Side.BlackHat;

		// If the blackhats already have a pimary player, then the requesting player becomes an advisor
		if(blackHatPrimaryPlayer is object)
			player.role = Networking.Player.Role.Advisor;
		// Otherwise... the requesting player becomes the blackhat primary player
		else if(player.role == Networking.Player.Role.Spectator)
			player.role = Networking.Player.Role.Player;

		// Synchronize the room state
		RequestRoomStateSynchronization();
	}

	// Function which causes the current player to become a spectator
	// Optionally a specific side can be passed, a spectator can only see information that can be seen by the side they are assigned to (common side can spectate both)
	public void BecomeSpectator(Networking.Player.Side side = Networking.Player.Side.Common){ photonView.RPC("RPC_NetworkingManager_BecomeSpectator", RpcTarget.AllBuffered, localPlayer.actorNumber, side); }
	[PunRPC] void RPC_NetworkingManager_BecomeSpectator(int playerActorNumber, Networking.Player.Side side){
		// Room state management can only be preformed by the host
		if(!isHost) return;

		// Find the player who requested to to become a spector
		Networking.Player player = players.Find(p => p.actorNumber == playerActorNumber);
		// Take the side as input
		player.side = side;
		// Make the requesting player a spectator
		player.role = Networking.Player.Role.Spectator;

		// Synchronize the room state
		RequestRoomStateSynchronization();
	}

	// Function which causes the current player to become their side's primary player, ensuring that each side only has one primary player
	public void BecomePrimaryPlayer(){ photonView.RPC("RPC_NetworkingManager_BecomePrimaryPlayer", RpcTarget.AllBuffered, localPlayer.actorNumber); }
	[PunRPC] void RPC_NetworkingManager_BecomePrimaryPlayer(int playerActorNumber){
		// Room state management can only be preformed by the host
		if(!isHost) return;

		// Find the player who wishes to become their side's primary player
		Networking.Player player = players.Find(p => p.actorNumber == playerActorNumber);

		// Get a reference to both side's primary player
		var whiteHatPrimaryPlayer = NetworkingManager.whiteHatPrimaryPlayer;
		var blackHatPrimaryPlayer = NetworkingManager.blackHatPrimaryPlayer;

		// If the player is a whitehat...
		if(player.side == Networking.Player.Side.WhiteHat){
			// And there is currently a primary whitehat player...
			if(whiteHatPrimaryPlayer is object)
				// The current whitehat primary player becomes an advisor
				whiteHatPrimaryPlayer.role = Networking.Player.Role.Advisor;
		// If the player is a blackhat...
		} else if(player.side == Networking.Player.Side.BlackHat)
			// And there is currently a primary blackhat player...
			if(blackHatPrimaryPlayer is object)
				// The current blackhat primary player becomes an advisor
				blackHatPrimaryPlayer.role = Networking.Player.Role.Advisor;

		// Make the requesting player their side's primary player
		player.role = Networking.Player.Role.Player;

		// Synchronize the room state
		RequestRoomStateSynchronization();
	}

	// Function which causes the current player to become an advisor
	public void BecomeAdvisor(){ photonView.RPC("RPC_NetworkingManager_BecomeAdvisor", RpcTarget.AllBuffered, localPlayer.actorNumber); }
	[PunRPC] void RPC_NetworkingManager_BecomeAdvisor(int playerActorNumber){
		// Room state management can only be preformed by the host
		if(!isHost) return;

		// Find the player who requested to become an advisor
		Networking.Player player = players.Find(p => p.actorNumber == playerActorNumber);
		// Mark them as an advisor
		player.role = Networking.Player.Role.Advisor;

		// Synchronize the room state
		RequestRoomStateSynchronization();
	}


	// --  Room State Synchronization --


	// Request that the host synchronize the room state
	// If a joining player is passed as input, then that player will also have the join room event triggered once the state is synchronized
	public void RequestRoomStateSynchronization(Player joinedPlayer = null){ photonView.RPC("RPC_NetworkingManager_RequestRoomStateSynchronization", RpcTarget.AllBuffered, joinedPlayer); }
	[PunRPC] void RPC_NetworkingManager_RequestRoomStateSynchronization(Player joinedPlayer){
		// Only the host can provide the room state
		if(!isHost) return;

		photonView.RPC("RPC_NetworkingManager_RoomStateSynchronization", RpcTarget.AllBuffered, players.ToArray(), joinedPlayer);
	}

	// Callback which sends a copy of the host's room state to all of the other players
	[PunRPC] void RPC_NetworkingManager_RoomStateSynchronization(Networking.Player[] _players, Player joinedPlayer){
		// Update the player list
		debuggingPlayers = players = new List<Networking.Player>(_players);
		// Find the local player in the updated player list
		Networking.Player.localPlayer = players.Find(p => p == PhotonNetwork.LocalPlayer);

		// If a player should have the join room event triggered, and we are that player... trigger the join room event
		if(joinedPlayer is object && joinedPlayer == Networking.Player.localPlayer)
			roomJoinEvent?.Invoke();

		// Trigger the room state update event
		roomStateUpdateEvent?.Invoke();
	}


	// -- Ready Functions


	// Check if all players have readied up
	public bool allPlayersReady(){
		foreach(Networking.Player p in players)
			if(!p.isReady) return false;

		return true;
	}

	// Marks the local player as being ready or not based on what is passed in
	public void setReady(bool isReady){
		localPlayer.photonPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable(){
			{IS_PLAYER_READY, isReady}
		});
	}

	// Toggles weather or not the local player is ready.
	public void toggleReady(){ setReady( !localPlayer.isReady ); }


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

	// Returns an array containing all of the whitehat players (primaries, advisors, and spectators)
	public static Networking.Player[] whiteHatPlayers {
		get {
			if(players.Count == 0) return null;
			return players.FindAll(p => p.side == Networking.Player.Side.WhiteHat).ToArray();
		}
	}

	// Returns an array containing all of the blackhat players (primaries, advisors, and spectators)
	public static Networking.Player[] blackHatPlayers {
		get {
			if(players.Count == 0) return null;
			return players.FindAll(p => p.side == Networking.Player.Side.BlackHat).ToArray();
		}
	}

	// Returns an array containing all of the common players (spectators)
	public static Networking.Player[] commonPlayers {
		get {
			if(players.Count == 0) return null;
			return players.FindAll(p => p.side == Networking.Player.Side.Common).ToArray();
		}
	}

	// Returns an array containing all of the spectators (whitehat, blackhat, and common)
	public static Networking.Player[] spectators {
		get {
			if(players.Count == 0) return null;
			return players.FindAll(p => p.role == Networking.Player.Role.Spectator).ToArray();
		}
	}

	// Returns a reference to the whitehat's current primary player (returns null if they don't exist)
	public static Networking.Player whiteHatPrimaryPlayer {
		get {
			var tmp = whiteHatPlayers;
			if(tmp is null) return null;

			return System.Array.Find(tmp, p => p.role == Networking.Player.Role.Player);
		}
	}

	// Returns an array containing all the whitehat advisors
	public static Networking.Player[] whiteHatAdvisors {
		get {
			var tmp = whiteHatPlayers;
			if(tmp is null) return null;

			return System.Array.FindAll(tmp, p => p.role == Networking.Player.Role.Advisor);
		}
	}

	// Returns a reference to the blackhat's current primary player (returns null if they don't exist)
	public static Networking.Player blackHatPrimaryPlayer {
		get {
			var tmp = blackHatPlayers;
			if(tmp is null) return null;

			return System.Array.Find(tmp, p => p.role == Networking.Player.Role.Player);
		}
	}

	// Returns an array containing all the blackhat advisors
	public static Networking.Player[] blackHatAdvisors {
		get {
			var tmp = blackHatPlayers;
			if(tmp is null) return null;

			return System.Array.FindAll(tmp, p => p.role == Networking.Player.Role.Advisor);
		}
	}

	// Returns true if we are a whitehat player
	public static bool isWhiteHat {
		get {
			if(Networking.Player.localPlayer is null) return false;
			return Networking.Player.localPlayer.side == Networking.Player.Side.WhiteHat;
		}
	}

	// Returns true if we are the primary whitehat player
	public static bool isWhiteHatPrimary {
		get {
			if(Networking.Player.localPlayer is null || whiteHatPrimaryPlayer is null) return false;
			return Networking.Player.localPlayer == whiteHatPrimaryPlayer;
		}
	}

	// Returns true if we are a blackhat player
	public static bool isBlackHat {
		get {
			if(Networking.Player.localPlayer is null) return false;
			return Networking.Player.localPlayer.side == Networking.Player.Side.BlackHat;
		}
	}

	// Returns true if we are the primary blackhat player
	public static bool isBlackHatPrimary {
		get {
			if(Networking.Player.localPlayer is null || blackHatPrimaryPlayer is null) return false;
			return Networking.Player.localPlayer == blackHatPrimaryPlayer;
		}
	}

	// Returns true if we are a spectator
	public static bool isSpectator {
		get {
			if(Networking.Player.localPlayer is null) return false;
			return Networking.Player.localPlayer.role == Networking.Player.Role.Spectator;
		}
	}

	// Returns true if we are currently in a room
	public static bool inRoom {
		get => PhotonNetwork.CurrentRoom is object;
	}

	// Returns true if the room is currently at its maximum capacity
	public static bool isRoomFull {
		get => PhotonNetwork.CurrentRoom.MaxPlayers == NetworkingManager.roomPlayers.Length;
	}

	// Alias to Networking.Player.localPlayer
	public static Networking.Player localPlayer {
		get => Networking.Player.localPlayer;
	}

	// Returns true if we are the primary player on our side
	public static bool isPrimary {
		get {
			if(localPlayer is null) return false;

			if(localPlayer.side == Networking.Player.Side.WhiteHat) return isWhiteHatPrimary;
			else if(localPlayer.side == Networking.Player.Side.BlackHat) return isBlackHatPrimary;
			else return false;
		}
	}

	// Returns true if the local player is marked as ready
	public static bool isReady {
		get {
			if(localPlayer is null) return false;
			return localPlayer.isReady;
		}
	}

	// Returns the current list of players
	public static Networking.Player[] roomPlayers {
		get {
			if(players is null) return null;
			return players.ToArray();
		}
	}
}
