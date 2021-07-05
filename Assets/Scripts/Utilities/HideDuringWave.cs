using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Class which disables the attached game object during waves
public class HideDuringWave : MonoBehaviour {
    void Awake(){
		GameManager.waveStartEvent += OnWaveStart;
		GameManager.waveEndEvent += OnWaveEnd;
	}

	void OnDestroy(){ // Disabled on destroy, since we still want the objects to be listening to the wave end event while disabled
		GameManager.waveStartEvent -= OnWaveStart;
		GameManager.waveEndEvent -= OnWaveEnd;
	}

	// When a wave starts disable this object
	void OnWaveStart(){ gameObject.SetActive(false); }
	// When a wave ends enable this object
	void OnWaveEnd(){ gameObject.SetActive(true); }
}
