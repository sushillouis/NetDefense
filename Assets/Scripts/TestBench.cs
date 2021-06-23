using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBench : MonoBehaviour {
	public PathNodeBase target;

    // Start is called before the first frame update
    void Start() {
        PathNodeBase pnb = GetComponent<PathNodeBase>();

		var path = pnb.findPathTo(target);

		string debug = "";
		foreach(var c in path)
			debug += c + ", ";
		Debug.Log(debug);
    }

}
