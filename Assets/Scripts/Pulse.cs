using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    public float pulseMagnitude;
    public float pulseSpeed;

    void Update()
    {
        float value = Mathf.PingPong(Time.time * pulseSpeed, pulseMagnitude);
        transform.localScale = new Vector3(1 + value, 1 + value, 1);
    }
}
