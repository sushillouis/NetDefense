using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UI elements that are common to both hats
public class BaseUI : Core.Utilities.Singleton<BaseUI> {
	// References to the score text UI elements
	public TMPro.TextMeshProUGUI blackHatScoreText, whiteHatScoreText;

	// Reference to the ready button's text
	public TMPro.TextMeshProUGUI readyText;
	// Reference to the Game Over screen
	public GameObject endGameScreen;
	// References to the win and lose text in the game over screen
	public GameObject winText, loseText;

	// Passthrough callbacks which redirect button presses to the game manager
	public void OnToggleReady(){ GameManager.instance.toggleReady(); }
	public void OnDisconnectButtonPressed(){ GameManager.instance.OnDisconnectButtonPressed(); }
}
