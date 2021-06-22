using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundSettings : MonoBehaviour {
	public GameObject settingsPannel;
	public Slider musicVolume, soundFXVolume;
	public AudioMixer masterMixer;

	void Start(){
		masterMixer.GetFloat("musicVol", out float mvol);
		masterMixer.GetFloat("sfxVol", out float svol);

		musicVolume.value = mvol;
		soundFXVolume.value = svol;
	}

	public void ToggleSettings(){
		Debug.Log(settingsPannel.activeSelf);
		if(settingsPannel.activeSelf)
			settingsPannel.SetActive(false);
		else
			settingsPannel.SetActive(true);
	}

	public void MusicVolumeChanged(){
		masterMixer.SetFloat("musicVol", musicVolume.value);
	}

	public void SoundFXVolumeChanged(){
		masterMixer.SetFloat("sfxVol", soundFXVolume.value);
	}

}
