using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Class which disables the attached game object after the game ends
public class HideAfterGameEnds : MonoBehaviour {
    void Awake(){ GameManager.gameEndEvent += OnGameEnd; }
	// Disabled on destroy, since we still want the objects to be listening to the wave end event while disabled
	void OnDestroy(){ GameManager.gameEndEvent -= OnGameEnd; }

	// When a wave starts disable this object
	void OnGameEnd(){ gameObject.SetActive(false); }
}
