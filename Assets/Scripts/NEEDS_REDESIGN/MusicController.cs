using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {
	public static MusicController inst;

    private void Awake() {
		inst = this;
		DontDestroyOnLoad(gameObject);
	}
}
