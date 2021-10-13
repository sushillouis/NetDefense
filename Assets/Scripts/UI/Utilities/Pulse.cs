using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component which slowly pulsates the size of the attached object
public class Pulse : MonoBehaviour {
	// How much the object should pulsate
    public float pulseMagnitude;
	// How quickly the object should pulsate
    public float pulseSpeed;

    void Update() {
        float value = Mathf.PingPong(Time.time * pulseSpeed, pulseMagnitude);
        transform.localScale = new Vector3(1 + value, 1 + value, 1);
    }
}
