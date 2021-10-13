using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component which manages the MainMenu
public class MainMenuManager : MonoBehaviour {
	// Reference to the lobby window prefab
	public GameObject lobbyPrefab;
	// Reference to the UI canvas
	public Canvas canvas;

	// Function called when the singleplayer button is pressed
	public void OnSingleplayerButtonPressed(){
		var instance = findOrCreateLobbyWindow();

		instance.OnLeaveRoomButtonPressed(); // Make sure that we leave any room so that we can join a new single player room
		instance.OnCreateSingleplayerRoomButtonPressed();
	}

	// Function called when the multiplayer button is pressed
	public void OnMultiplayerButtonPressed(){
		var instance = findOrCreateLobbyWindow();

		instance.titlebar.SetWindowTitle("Multiplayer");
		instance.OnLeaveRoomButtonPressed(); // Make sure that we leave any room so we are presented with the room list
	}

	// Function called when the quit button is pressed
	public void OnQuitPressed() {
		// Destroy(MusicController.inst.gameObject); // Destroy the music audio source so that in WebGL the music will stop
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	// Function which either finds the existing lobby window, or creates a new one if it doesn't exist
	LobbyScreenManager findOrCreateLobbyWindow(){
		// Try to find the lobby window
		LobbyScreenManager instance = GameObject.FindGameObjectWithTag("LobbyScreenWindow")?.GetComponent<LobbyScreenManager>();
		// Create it if not found
		if(instance is null){
			instance = Instantiate(lobbyPrefab, Vector3.zero, Quaternion.identity, canvas.transform).GetComponent<LobbyScreenManager>();
			instance.CenterOnScreen();
		}

		return instance;
	}

}
