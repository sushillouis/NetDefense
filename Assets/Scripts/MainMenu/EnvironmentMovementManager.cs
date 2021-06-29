using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentMovementManager : MonoBehaviour {
	// The environment that needs to move
	public GameObject[] environmentUnits;
	// The speed at which the environment should move
    public float movementSpeed = 45;

    // Update is called once per frame
	void Update() {
        foreach (GameObject unit in environmentUnits) {
            unit.transform.position -= new Vector3(0, 0, Time.deltaTime * movementSpeed);
            if (unit.transform.position.z < -400) {
                unit.transform.position += new Vector3(0, 0, 1600);
            }
        }
    }
}
