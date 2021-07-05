using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class LobbyScreenManager : MonoBehaviour {
	// The three screens that the lobby manages
    public GameObject roomListScreen, inRoomScreen, loadingPrompt;

	// RoomList
	[Header("Room List")]
	public Toggle multiplayerToggle;
	public Button createRoomButton;
	public TMPro.TMP_InputField aliasTextbox;
	public TMPro.TextMeshProUGUI[] roomLabels;
	public Button[] joinButtons;

	// In Room
	[Header("In Room")]
	public Toggle whiteHatToggle;
	public Button startButton, leaveButton;
	public TMPro.TextMeshProUGUI[] playerLabels;

	// Connect and disconnect to networking events
	void OnEnable(){
		NetworkingManager.connectedEvent += init; // Run the 'init' function once we are connected to the network!
		NetworkingManager.roomListUpdateEvent += updateRoomList; // Update the room list whenever one becomes available
		NetworkingManager.roomJoinEvent += joinedRoom; // Run join room function when we  join the room
		NetworkingManager.roomLeaveEvent += init; // Run the 'init' function when someone leaves the room
		NetworkingManager.roomOtherJoinEvent += playerJoined; // Run player joined when someone else joins
		NetworkingManager.roomOtherLeaveEvent += playerLeft; // Run player left when someone else leaves
		NetworkingManager.roomPropertiesUpdateEvent += roomPropertiesChanged; // Update the room properties whenever they change
		NetworkingManager.roomPlayerPropertiesUpdateEvent += roomPlayerPropertiesChanged; // Update the player properties whenever they change
	}
	void OnDisable(){
		NetworkingManager.connectedEvent -= init;
		NetworkingManager.roomListUpdateEvent -= updateRoomList;
		NetworkingManager.roomJoinEvent -= joinedRoom;
		NetworkingManager.roomLeaveEvent -= init;
		NetworkingManager.roomOtherJoinEvent -= playerJoined;
		NetworkingManager.roomOtherLeaveEvent -= playerLeft;
		NetworkingManager.roomPropertiesUpdateEvent -= roomPropertiesChanged;
		NetworkingManager.roomPlayerPropertiesUpdateEvent -= roomPlayerPropertiesChanged;
	}

	// When we load make sure the menus are hidden and the loading screen is visible
	void Start(){
		roomListScreen.SetActive(false);
		inRoomScreen.SetActive(false);
		loadingPrompt.SetActive(true);
	}


	// -- Networking Callbacks --


	// Displays the main screen and makes sure the loading screen is gone
	void init(){
		roomListScreen.SetActive(true);
		inRoomScreen.SetActive(false);

		// If the loading text is still present... destroy it
		if(loadingPrompt != null){
			Destroy(loadingPrompt);
			loadingPrompt = null;
		}
	}

	// Updates the list of rooms that can be joined
	void updateRoomList(List<RoomInfo> roomList){
		int i = 0;
		foreach(var info in roomList){
			if(i >= 5) break;
			if(!info.IsOpen || !info.IsVisible || info.RemovedFromList) continue;

			TMPro.TextMeshProUGUI buttonText = joinButtons[i].transform.GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>();

			roomLabels[i].text = (i + 1) + ". " + info.Name;
			buttonText.text = "Join as Second Player";

			i++;
		}
	}

	// Updates internal state when we join a room
	void joinedRoom(){
		roomListScreen.SetActive(false);
		inRoomScreen.SetActive(true);

		try{
			// Disable the whitehat toggle if we aren't the room's host
			if(!NetworkingManager.isHost)
				whiteHatToggle.gameObject.SetActive(false);
			else whiteHatToggle.gameObject.SetActive(true);

			// The host is always ready
			if(NetworkingManager.isHost && !NetworkingManager.isSingleplayer)
				NetworkingManager.localPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable(){
					{NetworkingManager.IS_PLAYER_READY, true}
				});
		} catch (System.NullReferenceException) { if(!NetworkingManager.isSingleplayer) throw; /*Rethrow exception if we aren't in singleplayer*/ }

		// Set that the main player is the whitehat by default (and propagate that change over the network)
		whiteHatToggle.isOn = true;
		OnWhitehatToggleStateChanged(true);

		updateStartButton();
	}

	// Updates the player status text as players join and leave the room, or when the player who is the whitehat changes
	void roomPropertiesChanged(ExitGames.Client.Photon.Hashtable _){
		if(NetworkingManager.whiteHatPlayerIndex != -1)
			playerLabels[0].text = "White: " + (NetworkingManager.isSingleplayer ? NetworkingManager.localPlayer.NickName : NetworkingManager.roomPlayers[NetworkingManager.whiteHatPlayerIndex].NickName);
		else playerLabels[0].text = "White: " + (!NetworkingManager.isSingleplayer ? "Joining..." : "AI");

		if(NetworkingManager.blackHatPlayerIndex != -1)
			playerLabels[1].text = "Black: " + (NetworkingManager.isSingleplayer ? NetworkingManager.localPlayer.NickName : NetworkingManager.roomPlayers[NetworkingManager.blackHatPlayerIndex].NickName);
		else playerLabels[1].text = "Black: " + (!NetworkingManager.isSingleplayer ? "Joining..." : "AI");
	}

	// Makes sure the start game button state is updated as players ready and unready
	void roomPlayerPropertiesChanged(Player _, ExitGames.Client.Photon.Hashtable _1){
		// Make sure the start button state is synced
		updateStartButton();
	}

	// Makes sure state is synced when a player
	void playerJoined(Player _){
		// Make sure the start button state is synced
		updateStartButton();
		// Make sure the shared state is synced
		OnWhitehatToggleStateChanged(whiteHatToggle.isOn);
	}

	// When the host leaves a room the other player should leave the room as well
	void playerLeft(Player _){
		// If the host left then we leave the room as well
		if(!NetworkingManager.isHost){
			NetworkingManager.instance.LeaveRoom();
			init();
		}
	}


	// -- UI Callbacks --


	// Function called whenever the create room button is pressed, it updates the player's name and creates a room
	public void OnCreateRoomButtonPressed(){
		updatePlayerAlias();

		if(multiplayerToggle.isOn)
			NetworkingManager.instance.CreateRoom(2, true);
		else
			NetworkingManager.instance.CreateOfflineRoom();
	}

	// Function called when one of the join room buttons is pressed (it joins the specified room)
	public void OnJoinRoomButtonPressed(int index){
		updatePlayerAlias();

		string name = roomLabels[index].text.Split('.')[1].Trim();
		NetworkingManager.instance.JoinRoom(name);
	}

	// Function called when the WhiteHat toggle is pressed, makes the host player the whitehat or blackhat depending
	public void OnWhitehatToggleStateChanged(bool state){
		if(multiplayerToggle.isOn)
			// Update the room's properties to reflect the new hat state
			NetworkingManager.instance.SetRoomProperties(new ExitGames.Client.Photon.Hashtable(){
				{NetworkingManager.IS_HOST_WHITE_HAT, state} // hat
			});
		else {
			NetworkingManager.whiteHatPlayerIndex = state ? 0 : -1;
			NetworkingManager.blackHatPlayerIndex = state ? -1 : 0;

			// Ensure that the player status text at the button is updated
			roomPropertiesChanged(new ExitGames.Client.Photon.Hashtable());
		}
	}

	// Function called when the start game button is pressed (if we aren't the multiplayer host this is the ready/unready button)
	public void OnStartGameButtonPressed(){
		// If we aren't the host then toggle our ready state
		if(!NetworkingManager.isHost){
			NetworkingManager.instance.toggleReady();

			updateStartButton();
			return;
		}

		// Load the gameplay scene
		SceneManager.LoadScene(1);
	}

	// Function called when the leave room button is pressed
	public void OnLeaveRoomButtonPressed(){
		if(multiplayerToggle.isOn)
			NetworkingManager.instance.LeaveRoom();
		else
			NetworkingManager.instance.Reconnect();
	}


	// -- Helpers --


	// Utility function which updates the player's name on the network
	 void updatePlayerAlias(){
		string playerAlias = aliasTextbox.text;
		if(string.IsNullOrEmpty(playerAlias)) playerAlias = "You";
		NetworkingManager.localPlayer.NickName = playerAlias; // Update our nickname in photon
	}

	// Helper function which updates the start button's text and intractability to reflect the current state of the lobby
	void updateStartButton(){
		TMPro.TextMeshProUGUI buttonText = startButton.transform.GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>();
		// If we aren't the host defer to the host to press the start button (the button becomes a ready button)
		if(!NetworkingManager.isHost && !NetworkingManager.instance.isPlayerReady(NetworkingManager.localPlayer)){
			startButton.interactable = true;
			buttonText.text = "Ready Up!";
			return;
		} else if(!NetworkingManager.isHost){
			startButton.interactable = true;
			buttonText.text = "Unready";
			return;
		}

		// Update the start button to reflect the number of players in the room
		if(NetworkingManager.isRoomFull && !NetworkingManager.isSingleplayer){
			if(NetworkingManager.instance.allPlayersReady()){
				startButton.interactable = true;
				buttonText.text = "Start";
			} else {
				startButton.interactable = false;
				buttonText.text = "Waiting for Ready...";
			}
		} else if(!NetworkingManager.isSingleplayer) {
			startButton.interactable = false;
			buttonText.text = "Waiting for Players...";
		} else {
			startButton.interactable = true;
			buttonText.text = "Start Singleplayer";
		}
	}
}
