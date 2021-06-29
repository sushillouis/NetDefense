using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepBetweenScenes : MonoBehaviour {
    private void Awake() { DontDestroyOnLoad(gameObject); }
}
