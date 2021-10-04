using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

    // Update is called once per frame
    void Update() {
		// Make sure the global scale remains constant (set by unparenting and then reparenting)
		Transform parent = transform.parent;
		transform.parent.SetParent(null, false);
		transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
		transform.SetParent(parent, true);

		// Make sure the object is always rotated so that it is facing the camera
		transform.LookAt(Camera.main.transform);
    }
}
